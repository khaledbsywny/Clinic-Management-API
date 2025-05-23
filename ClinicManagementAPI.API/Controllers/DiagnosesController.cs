using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ClinicManagementAPI.Application.Common.DTOs;
using ClinicManagementAPI.Application.Common.Interfaces;
using ClinicManagementAPI.Domain.Entities;

namespace ClinicManagementAPI.API.Controllers;

[Authorize]
public class DiagnosesController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DiagnosesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<IEnumerable<DiagnosisDto>>> GetAll()
    {
        var diagnoses = await _unitOfWork.Repository<Diagnosis>().GetAllAsync();
        var diagnosisDtos = _mapper.Map<IEnumerable<DiagnosisDto>>(diagnoses);
        return Ok(diagnosisDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DiagnosisDto>> GetById(int id)
    {
        var diagnosis = await _unitOfWork.Repository<Diagnosis>().GetByIdAsync(id);
        if (diagnosis == null)
            return NotFound();

        // Check if the user has permission to view this diagnosis
        if (!await HasPermissionToViewDiagnosis(diagnosis))
            return Forbid();

        return HandleResult(_mapper.Map<DiagnosisDto>(diagnosis));
    }

    [HttpPost]
    [Authorize(Roles = "Doctor")]
    public async Task<ActionResult<DiagnosisDto>> Create(CreateDiagnosisDto model)
    {
        var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(model.AppointmentId);
        if (appointment == null)
            return NotFound("Appointment not found");

        // Check if the doctor is assigned to this appointment
        if (appointment.DoctorId != GetCurrentUserId())
            return Forbid();

        var diagnosis = _mapper.Map<Diagnosis>(model);
        await _unitOfWork.Repository<Diagnosis>().AddAsync(diagnosis);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = diagnosis.Id }, _mapper.Map<DiagnosisDto>(diagnosis));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Update(int id, UpdateDiagnosisDto model)
    {
        var diagnosis = await _unitOfWork.Repository<Diagnosis>().GetByIdAsync(id);
        if (diagnosis == null)
            return NotFound();

        // Check if the doctor is assigned to this diagnosis's appointment
        if (!await HasPermissionToUpdateDiagnosis(diagnosis))
            return Forbid();

        _mapper.Map(model, diagnosis);
        await _unitOfWork.Repository<Diagnosis>().UpdateAsync(diagnosis);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var diagnosis = await _unitOfWork.Repository<Diagnosis>().GetByIdAsync(id);
        if (diagnosis == null)
            return NotFound();

        await _unitOfWork.Repository<Diagnosis>().DeleteAsync(diagnosis);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("appointment/{appointmentId}")]
    public async Task<ActionResult<DiagnosisDto>> GetByAppointment(int appointmentId)
    {
        var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(appointmentId);
        if (appointment == null)
            return NotFound("Appointment not found");

        // Check if the user has permission to view this appointment's diagnosis
        if (!await HasPermissionToViewAppointment(appointment))
            return Forbid();

        var diagnosis = (await _unitOfWork.Repository<Diagnosis>().GetAllAsync())
            .FirstOrDefault(d => d.AppointmentId == appointmentId);

        return HandleResult(_mapper.Map<DiagnosisDto>(diagnosis));
    }

    private async Task<bool> HasPermissionToViewDiagnosis(Diagnosis diagnosis)
    {
        var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(diagnosis.AppointmentId);
        return await HasPermissionToViewAppointment(appointment!);
    }

    private async Task<bool> HasPermissionToViewAppointment(Appointment appointment)
    {
        if (User.IsInRole("Admin"))
            return true;

        if (User.IsInRole("Doctor"))
            return appointment.DoctorId == GetCurrentUserId();

        if (User.IsInRole("Patient"))
            return appointment.PatientId == GetCurrentUserId();

        return false;
    }

    private async Task<bool> HasPermissionToUpdateDiagnosis(Diagnosis diagnosis)
    {
        var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(diagnosis.AppointmentId);
        return appointment?.DoctorId == GetCurrentUserId();
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User is not authenticated");

        return int.Parse(userIdClaim.Value);
    }
} 
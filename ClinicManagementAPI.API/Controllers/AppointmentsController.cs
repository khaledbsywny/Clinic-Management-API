using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ClinicManagementAPI.Application.Common.DTOs;
using ClinicManagementAPI.Application.Common.Interfaces;
using ClinicManagementAPI.Domain.Entities;
using ClinicManagementAPI.Domain.Enums;

namespace ClinicManagementAPI.API.Controllers;

[Authorize]
public class AppointmentsController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AppointmentsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAll()
    {
        var appointments = await _unitOfWork.Repository<Appointment>().GetAllAsync();
        var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        return Ok(appointmentDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDto>> GetById(int id)
    {
        var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(id);
        return HandleResult(_mapper.Map<AppointmentDto>(appointment));
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> Create(CreateAppointmentDto model)
    {
        var appointment = _mapper.Map<Appointment>(model);
        appointment.Status = AppointmentStatus.Pending;
        appointment.PatientId = GetCurrentUserId(); // Get from claims

        await _unitOfWork.Repository<Appointment>().AddAsync(appointment);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, _mapper.Map<AppointmentDto>(appointment));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateAppointmentDto model)
    {
        var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(id);
        if (appointment == null)
            return NotFound();

        // Only allow status updates for doctors and admins
        if (User.IsInRole("Patient") && model.Status != appointment.Status)
            return Forbid();

        _mapper.Map(model, appointment);
        await _unitOfWork.Repository<Appointment>().UpdateAsync(appointment);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var appointment = await _unitOfWork.Repository<Appointment>().GetByIdAsync(id);
        if (appointment == null)
            return NotFound();

        // Only allow patients to cancel their own appointments
        if (User.IsInRole("Patient") && appointment.PatientId != GetCurrentUserId())
            return Forbid();

        await _unitOfWork.Repository<Appointment>().DeleteAsync(appointment);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("doctor/{doctorId}")]
    [Authorize(Roles = "Doctor,Admin")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetByDoctor(int doctorId)
    {
        var appointments = await _unitOfWork.Repository<Appointment>().GetAllAsync();
        var doctorAppointments = appointments.Where(a => a.DoctorId == doctorId);
        var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(doctorAppointments);
        return Ok(appointmentDtos);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetByPatient(int patientId)
    {
        // Only allow patients to view their own appointments
        if (User.IsInRole("Patient") && patientId != GetCurrentUserId())
            return Forbid();

        var appointments = await _unitOfWork.Repository<Appointment>().GetAllAsync();
        var patientAppointments = appointments.Where(a => a.PatientId == patientId);
        var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(patientAppointments);
        return Ok(appointmentDtos);
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User is not authenticated");

        return int.Parse(userIdClaim.Value);
    }
} 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ClinicManagementAPI.Application.Common.DTOs;
using ClinicManagementAPI.Application.Common.Interfaces;
using ClinicManagementAPI.Domain.Entities;

namespace ClinicManagementAPI.API.Controllers;

[Authorize]
public class PatientsController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PatientsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
    {
        var patients = await _unitOfWork.Repository<Patient>().GetAllAsync();
        var patientDtos = _mapper.Map<IEnumerable<PatientDto>>(patients);
        return Ok(patientDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDto>> GetById(int id)
    {
        var patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(id);
        return HandleResult(_mapper.Map<PatientDto>(patient));
    }

    [HttpPost]
    public async Task<ActionResult<PatientDto>> Create(CreatePatientDto model)
    {
        var patient = _mapper.Map<Patient>(model);
        await _unitOfWork.Repository<Patient>().AddAsync(patient);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, _mapper.Map<PatientDto>(patient));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdatePatientDto model)
    {
        var patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(id);
        if (patient == null)
            return NotFound();

        _mapper.Map(model, patient);
        await _unitOfWork.Repository<Patient>().UpdateAsync(patient);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(id);
        if (patient == null)
            return NotFound();

        await _unitOfWork.Repository<Patient>().DeleteAsync(patient);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
} 
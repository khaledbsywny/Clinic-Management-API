using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ClinicManagementAPI.Application.Common.DTOs;
using ClinicManagementAPI.Application.Common.Interfaces;
using ClinicManagementAPI.Domain.Entities;

namespace ClinicManagementAPI.API.Controllers;

[Authorize]
public class DoctorsController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DoctorsController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAll()
    {
        var doctors = await _unitOfWork.Repository<Doctor>().GetAllAsync();
        var doctorDtos = _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        return Ok(doctorDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DoctorDto>> GetById(int id)
    {
        var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(id);
        return HandleResult(_mapper.Map<DoctorDto>(doctor));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<DoctorDto>> Create(CreateDoctorDto model)
    {
        var doctor = _mapper.Map<Doctor>(model);
        await _unitOfWork.Repository<Doctor>().AddAsync(doctor);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = doctor.Id }, _mapper.Map<DoctorDto>(doctor));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateDoctorDto model)
    {
        var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(id);
        if (doctor == null)
            return NotFound();

        _mapper.Map(model, doctor);
        await _unitOfWork.Repository<Doctor>().UpdateAsync(doctor);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(id);
        if (doctor == null)
            return NotFound();

        await _unitOfWork.Repository<Doctor>().DeleteAsync(doctor);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
} 
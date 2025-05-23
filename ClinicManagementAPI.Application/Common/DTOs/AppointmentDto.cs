using ClinicManagementAPI.Domain.Enums;

namespace ClinicManagementAPI.Application.Common.DTOs;

public class AppointmentDto : BaseDto
{
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public DoctorDto Doctor { get; set; } = null!;
    public PatientDto Patient { get; set; } = null!;
    public DiagnosisDto? Diagnosis { get; set; }
}

public class CreateAppointmentDto
{
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateAppointmentDto
{
    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
} 
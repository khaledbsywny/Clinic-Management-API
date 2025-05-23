using ClinicManagementAPI.Domain.Common;
using ClinicManagementAPI.Domain.Enums;

namespace ClinicManagementAPI.Domain.Entities;

public class Appointment : BaseEntity
{
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public Doctor Doctor { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
    public Diagnosis? Diagnosis { get; set; }
} 
using ClinicManagementAPI.Domain.Common;

namespace ClinicManagementAPI.Domain.Entities;

public class Diagnosis : BaseEntity
{
    public int AppointmentId { get; set; }
    public int DoctorId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string PrescribedMedications { get; set; } = string.Empty;
    public string? AdditionalNotes { get; set; }

    // Navigation properties
    public Appointment Appointment { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
} 
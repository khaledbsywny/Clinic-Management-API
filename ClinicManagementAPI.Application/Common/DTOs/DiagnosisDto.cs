namespace ClinicManagementAPI.Application.Common.DTOs;

public class DiagnosisDto : BaseDto
{
    public int AppointmentId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string PrescribedMedications { get; set; } = string.Empty;
    public string? AdditionalNotes { get; set; }
    public AppointmentDto Appointment { get; set; } = null!;
}

public class CreateDiagnosisDto
{
    public int AppointmentId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string PrescribedMedications { get; set; } = string.Empty;
    public string? AdditionalNotes { get; set; }
}

public class UpdateDiagnosisDto
{
    public string Description { get; set; } = string.Empty;
    public string PrescribedMedications { get; set; } = string.Empty;
    public string? AdditionalNotes { get; set; }
} 
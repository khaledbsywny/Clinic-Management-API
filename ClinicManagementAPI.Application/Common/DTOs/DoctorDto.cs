namespace ClinicManagementAPI.Application.Common.DTOs;

public class DoctorDto : BaseDto
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string ClinicAddress { get; set; } = string.Empty;
}

public class CreateDoctorDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string ClinicAddress { get; set; } = string.Empty;
}

public class UpdateDoctorDto
{
    public string Specialization { get; set; } = string.Empty;
    public string ClinicAddress { get; set; } = string.Empty;
} 
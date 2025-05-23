namespace ClinicManagementAPI.Application.Common.DTOs;

public abstract class BaseDto
{
    public string Id { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 
namespace ClinicManagementAPI.Application.Common.DTOs;

public class NotificationDto : BaseDto
{
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public UserDto User { get; set; } = null!;
}

public class CreateNotificationDto
{
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class UpdateNotificationDto
{
    public bool IsRead { get; set; }
} 
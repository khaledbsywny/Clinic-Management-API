using ClinicManagementAPI.Domain.Common;

namespace ClinicManagementAPI.Domain.Entities;

public class Notification : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
} 
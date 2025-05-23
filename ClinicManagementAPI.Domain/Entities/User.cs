using Microsoft.AspNetCore.Identity;
using ClinicManagementAPI.Domain.Common;
using ClinicManagementAPI.Domain.Enums;

namespace ClinicManagementAPI.Domain.Entities;

public class User : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation properties
    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
} 
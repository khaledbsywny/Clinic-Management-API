using ClinicManagementAPI.Domain.Common;

namespace ClinicManagementAPI.Domain.Entities;

public class Patient : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
} 
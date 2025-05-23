using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClinicManagementAPI.Domain.Common;

namespace ClinicManagementAPI.Domain.Entities;

public class Doctor : BaseEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = null!;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;

    [Required]
    public string Specialization { get; set; } = null!;

    [Required]
    public string LicenseNumber { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();
} 
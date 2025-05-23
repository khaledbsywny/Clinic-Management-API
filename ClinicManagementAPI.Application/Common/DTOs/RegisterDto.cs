using System.ComponentModel.DataAnnotations;
using ClinicManagementAPI.Domain.Enums;

namespace ClinicManagementAPI.Application.Common.DTOs;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = null!;

    [Required]
    public string FullName { get; set; } = null!;

    [Required]
    public UserRole Role { get; set; }
} 
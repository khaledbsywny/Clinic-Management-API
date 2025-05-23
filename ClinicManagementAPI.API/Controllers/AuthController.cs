using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClinicManagementAPI.Domain.Entities;
using ClinicManagementAPI.Domain.Enums;
using ClinicManagementAPI.Application.Common.DTOs;
using ClinicManagementAPI.Application.Common.Interfaces;

namespace ClinicManagementAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

    public AuthController(UserManager<User> userManager, IConfiguration configuration, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new User
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            Role = model.Role
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Assign role
            await _userManager.AddToRoleAsync(user, model.Role.ToString());

            // Create role-specific entity
            if (model.Role == UserRole.Doctor)
            {
                var doctor = new Doctor
                {
                    UserId = user.Id,
                    Specialization = "General", // Default value, can be updated later
                    LicenseNumber = "TEMP-" + Guid.NewGuid().ToString().Substring(0, 8) // Temporary license number
                };
                await _unitOfWork.Repository<Doctor>().AddAsync(doctor);
            }
            else if (model.Role == UserRole.Patient)
            {
                var patient = new Patient
                {
                    UserId = user.Id,
                    DateOfBirth = DateTime.UtcNow, // Default value, can be updated later
                    Gender = "Unknown" // Default value, can be updated later
                };
                await _unitOfWork.Repository<Patient>().AddAsync(patient);
            }

            await _unitOfWork.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token, User = new { user.Id, user.Email, user.FullName, user.Role } });
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Unauthorized(new { message = "Invalid email or password" });

        var result = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!result)
            return Unauthorized(new { message = "Invalid email or password" });

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token, User = new { user.Id, user.Email, user.FullName, user.Role } });
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
} 
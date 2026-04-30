using System.ComponentModel.DataAnnotations;

namespace MechanicalSheets.Api.DTOs;

public class RegisterDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(mechanic|manager)$", ErrorMessage = "Role must be mechanic or manager")]
    public string Role { get; set; } = "mechanic";
}
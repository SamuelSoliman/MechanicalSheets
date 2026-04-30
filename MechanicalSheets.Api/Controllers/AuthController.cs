using Microsoft.AspNetCore.Mvc;
using MechanicalSheets.Api.DTOs;
using MechanicalSheets.Api.Services;

namespace MechanicalSheets.Api.Controllers;


[ApiController]
[Route("api/auth")]  
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

   
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
    
        var token = await _authService.LoginAsync(dto);

        if (token == null)
           
            return Unauthorized(new { message = "Credenziali non valide" });

        return Ok(new { token });
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = await _authService.RegisterAsync(dto);

        if (user == null)
           
            return Conflict(new { message = "Email già registrata" });

    
        return CreatedAtAction(nameof(Login), new { id = user.Id, email = user.Email });
    }
}
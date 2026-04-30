using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using MechanicalSheets.Api.Data;
using MechanicalSheets.Api.DTOs;
using MechanicalSheets.Api.Models;

namespace MechanicalSheets.Api.Services;

public interface IAuthService
{
    Task<string?> LoginAsync(LoginDto dto);
    Task<User?> RegisterAsync(RegisterDto dto);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    
    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<string?> LoginAsync(LoginDto dto)
    {
       
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null) return null;

      
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            return null;

        return GenerateJwtToken(user);
    }

    public async Task<User?> RegisterAsync(RegisterDto dto)
    {
    
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return null;

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role
        };

        
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return user;
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
           
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expiresHours = int.Parse(_config["Jwt:ExpiresInHours"] ?? "8");

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiresHours),
            signingCredentials: credentials
        );

        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
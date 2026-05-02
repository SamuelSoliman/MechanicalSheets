using Microsoft.EntityFrameworkCore;
using MechanicalSheets.Api.Data;

namespace MechanicalSheets.Api.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKeyHeader = "X-Api-Key";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        
        if (context.Request.Path.StartsWithSegments("/api/integration"))
        {
            
            if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var providedKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { message = "API Key mancante" });
                return;
            }

            
            var keyHash = Convert.ToHexString(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(providedKey.ToString())
                )
            ).ToLower();

        
            var apiKey = await db.ApiKeys
                .FirstOrDefaultAsync(k => k.KeyHash == keyHash && k.IsActive);

            if (apiKey == null)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new { message = "API Key non valida" });
                return;
            }
        }

       
        await _next(context);
    }
}
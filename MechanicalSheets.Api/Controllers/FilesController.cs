using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace MechanicalSheets.Api.Controllers;

[ApiController]
[Route("api/files")]
[Authorize] 
public class FilesController : ControllerBase
{
    private readonly IConfiguration _config;

    public FilesController(IConfiguration config)
    {
        _config = config;
    }

   
    [HttpGet("{fileName}")]
    public IActionResult GetFile(string fileName)
    {

        var safeName = Path.GetFileName(fileName);
        if (string.IsNullOrEmpty(safeName))
            return BadRequest(new { message = "Nome file non valido" });

        var uploadPath = _config["Storage:UploadPath"] ?? "/app/uploads";
        var fullPath   = Path.Combine(uploadPath, safeName);

        if (!System.IO.File.Exists(fullPath))
            return NotFound(new { message = "File non trovato" });

       
        var ext = Path.GetExtension(safeName).ToLower();
        var contentType = ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png"            => "image/png",
            ".webp"           => "image/webp",
            _                 => "application/octet-stream"
        };

       
        var stream = System.IO.File.OpenRead(fullPath);
        return File(stream, contentType);
    }
}
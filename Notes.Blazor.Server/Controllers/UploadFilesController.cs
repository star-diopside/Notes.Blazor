using Microsoft.AspNetCore.Mvc;
using Notes.Blazor.Shared;
using System.Security.Cryptography;

namespace Notes.Blazor.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class UploadFilesController : ControllerBase
{
    private readonly ILogger<UploadFilesController> _logger;

    public UploadFilesController(ILogger<UploadFilesController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{id}")]
    public void Get(int id)
    {
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(IFormFile file)
    {
        byte[] hash;

        using (var sha256 = SHA256.Create())
        using (var stream = file.OpenReadStream())
        {
            hash = await sha256.ComputeHashAsync(stream);
        }

        var uploadFile = new UploadFile(file.FileName,
                                        file.ContentType,
                                        file.Length,
                                        string.Concat(hash.Select(b => b.ToString("x2"))));

        return CreatedAtAction(nameof(Get), new { id = 1 }, uploadFile);
    }
}

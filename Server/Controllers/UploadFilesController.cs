using Microsoft.AspNetCore.Mvc;

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
    public IActionResult Post(IFormFile file)
    {
        _logger.LogDebug("FileName: {FileName}, Length: {Length}, ContentType: {ContentType}",
                         file.FileName, file.Length, file.ContentType);
        return CreatedAtAction(nameof(Get), new { id = 1 }, null);
    }
}

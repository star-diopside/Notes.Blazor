using Microsoft.AspNetCore.Mvc;
using Notes.Blazor.Data.Models;
using Notes.Blazor.Data.Repositories;
using Notes.Blazor.Server.Models;
using Notes.Blazor.Shared;
using System.Security.Cryptography;

namespace Notes.Blazor.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class UserFilesController : ControllerBase
{
    private readonly IUserFileRepository _userFileRepository;

    public UserFilesController(IUserFileRepository userFileRepository)
    {
        _userFileRepository = userFileRepository;
    }

    [HttpGet]
    public IAsyncEnumerable<UploadedFile> IndexAsync()
    {
        return _userFileRepository.FindAllAsync(userFile => userFile.ToUploadedFile());
    }

    [HttpGet("{id}")]
    public async ValueTask<IActionResult> GetAsync(int id)
    {
        var userFile = await _userFileRepository.FindByIdAsync(id);

        if (userFile is null)
        {
            return NotFound();
        }

        return Ok(userFile.ToUploadedFile());
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

        var userFile = _userFileRepository.Add(new UserFile(
            fileName: file.FileName,
            length: file.Length,
            hashValue: string.Concat(hash.Select(b => b.ToString("x2"))))
        {
            ContentType = file.ContentType
        });
        await _userFileRepository.SaveChangesAsync();

        return CreatedAtAction("Get", new { id = userFile.Id }, userFile.ToUploadedFile());
    }
}

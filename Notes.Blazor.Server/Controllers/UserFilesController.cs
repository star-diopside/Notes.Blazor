using Microsoft.AspNetCore.Mvc;
using Notes.Blazor.Data.Models;
using Notes.Blazor.Data.Repositories;
using Notes.Blazor.Server.Models;
using System.Net.Mime;
using System.Security.Cryptography;
using X.PagedList;

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
    public async ValueTask<IActionResult> IndexAsync(int? pageNumber, int? pageSize)
    {
        if (pageSize.HasValue)
        {
            var uploadedFiles = await _userFileRepository.ListAsync(userFile => userFile.ToUploadedFile(),
                                                                    pageNumber ?? 1,
                                                                    pageSize.GetValueOrDefault());
            return Ok(uploadedFiles.ToPagesData());
        }
        else
        {
            return Ok(new { Data = _userFileRepository.ListAsync(userFile => userFile.ToUploadedFile()) });
        }
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

    [HttpGet("{id}/data")]
    public async Task<IActionResult> DownloadAsync(int id)
    {
        var userFile = await _userFileRepository.GetUserFileDataAsync(id);

        if (userFile?.UserFileData?.Data is null)
        {
            return NotFound();
        }

        return File(userFile.UserFileData.Data,
                    userFile.ContentType ?? MediaTypeNames.Application.Octet,
                    userFile.FileName);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(IFormFile file)
    {
        byte[] hash;
        byte[] data;

        using (var sha256 = SHA256.Create())
        using (var stream = file.OpenReadStream())
        {
            hash = await sha256.ComputeHashAsync(stream);
        }

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            data = stream.ToArray();
        }

        var userFile = _userFileRepository.Add(new UserFile(
            fileName: file.FileName,
            length: file.Length,
            hashValue: string.Concat(hash.Select(b => b.ToString("x2"))))
        {
            ContentType = file.ContentType,
            UserFileData = new()
            {
                Data = data
            }
        });
        await _userFileRepository.SaveChangesAsync();

        return CreatedAtAction("Get", new { id = userFile.Id }, userFile.ToUploadedFile());
    }
}

using Hnx8.ReadJEnc;
using Microsoft.AspNetCore.Components.Forms;

namespace Notes.Blazor.Client.ViewModels;

public class UploadFilesViewModel
{
    private readonly ILogger<UploadFilesViewModel> _logger;
    private readonly HttpClient _httpClient;

    public string? Text { get; set; }

    public UploadFilesViewModel(ILogger<UploadFilesViewModel> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task LoadFilesAsync(InputFileChangeEventArgs e)
    {
        try
        {
            _logger.LogDebug("File: {File}", new { e.File.Name, e.File.Size, e.File.ContentType, e.File.LastModified });

            using var multipart = new MultipartFormDataContent();
            var content = new StreamContent(e.File.OpenReadStream());
            if (!string.IsNullOrEmpty(e.File.ContentType))
            {
                content.Headers.ContentType = new(e.File.ContentType);
            }
            multipart.Add(content, "file", e.File.Name);
            var uploadTask = _httpClient.PostAsync("UploadFiles", multipart);

            Text = await ReadTextAsync(e.File);

            var response = await uploadTask;
            _logger.LogDebug("HttpResponseMessage: {Response}", response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    private async Task<string?> ReadTextAsync(IBrowserFile file)
    {
        byte[] array;

        using (var stream = file.OpenReadStream())
        using (var memory = new MemoryStream())
        {
            await stream.CopyToAsync(memory);
            array = memory.ToArray();
        }

        var code = ReadJEnc.JP.GetEncoding(array, array.Length, out string readText);

        if (code is null)
        {
            return null;
        }
        else
        {
            _logger.LogDebug("CharCode: {CharCode}", code);
            return readText;
        }
    }
}

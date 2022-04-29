using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using Notes.Blazor.Shared;
using System.Net.Http.Json;

namespace Notes.Blazor.Client.Services;

public class UploadFileService : IUploadFileService
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<NotesClientOptions> _options;

    public UploadFileService(HttpClient httpClient, IOptions<NotesClientOptions> options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public Task<UploadedFile[]?> ListAsync()
    {
        return _httpClient.GetFromJsonAsync<UploadedFile[]>("UserFiles");
    }

    public async Task<UploadedFile?> UploadFileAsync(IBrowserFile file)
    {
        using var multipart = new MultipartFormDataContent();
        var content = new StreamContent(file.OpenReadStream(_options.Value.UploadAllowedSize));
        if (!string.IsNullOrEmpty(file.ContentType))
        {
            content.Headers.ContentType = new(file.ContentType);
        }
        multipart.Add(content, "file", file.Name);

        using var response = await _httpClient.PostAsync("UserFiles", multipart).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UploadedFile>().ConfigureAwait(false);
    }
}

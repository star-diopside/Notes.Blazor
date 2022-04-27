using Microsoft.AspNetCore.Components.Forms;
using Notes.Blazor.Shared;
using System.Net.Http.Json;

namespace Notes.Blazor.Client.Services;

public class UploadFileService : IUploadFileService
{
    private readonly HttpClient _httpClient;

    public UploadFileService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<UploadedFile[]?> ListAsync()
    {
        return _httpClient.GetFromJsonAsync<UploadedFile[]>("UploadFiles");
    }

    public async Task<UploadedFile?> UploadFileAsync(IBrowserFile file)
    {
        using var multipart = new MultipartFormDataContent();
        var content = new StreamContent(file.OpenReadStream());
        if (!string.IsNullOrEmpty(file.ContentType))
        {
            content.Headers.ContentType = new(file.ContentType);
        }
        multipart.Add(content, "file", file.Name);

        using var response = await _httpClient.PostAsync("UploadFiles", multipart).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UploadedFile>().ConfigureAwait(false);
    }
}

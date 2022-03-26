using Microsoft.AspNetCore.Components.Forms;
using Notes.Blazor.Shared;
using System.Text.Json;

namespace Notes.Blazor.Client.Services;

public class HostService : IHostService
{
    private readonly HttpClient _httpClient;

    public HostService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UploadFile?> UploadFileAsync(IBrowserFile file)
    {
        using var multipart = new MultipartFormDataContent();
        var content = new StreamContent(file.OpenReadStream());
        if (!string.IsNullOrEmpty(file.ContentType))
        {
            content.Headers.ContentType = new(file.ContentType);
        }
        multipart.Add(content, "file", file.Name);

        var response = await _httpClient.PostAsync("UploadFiles", multipart);

        using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<UploadFile>(stream, options: new(JsonSerializerDefaults.Web));
    }
}

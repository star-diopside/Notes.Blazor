using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
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

    public Task<PagesData<UploadedFile>?> ListAsync(int? pageNumber = null)
    {
        var queryString = new Dictionary<string, string>();

        if (pageNumber.HasValue)
        {
            queryString["pageNumber"] = pageNumber.GetValueOrDefault().ToString();
        }

        if (_options.Value.PageSize.HasValue)
        {
            queryString["pageSize"] = _options.Value.PageSize.GetValueOrDefault().ToString();
        }

        var uri = QueryHelpers.AddQueryString("UserFiles", queryString);
        return _httpClient.GetFromJsonAsync<PagesData<UploadedFile>>(uri);
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

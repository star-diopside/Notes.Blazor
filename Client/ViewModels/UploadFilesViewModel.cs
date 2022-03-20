using Hnx8.ReadJEnc;
using Microsoft.AspNetCore.Components.Forms;
using System.Text;

namespace Notes.Blazor.Client.ViewModels;

public class UploadFilesViewModel
{
    private readonly ILogger<UploadFilesViewModel> _logger;
    private readonly HttpClient _httpClient;

    public IBrowserFile? SelectedFile { get; set; }
    public IEnumerable<EncodingInfo> Encodings { get; } = Encoding.GetEncodings();
    public int? EncodingCodePage { get; set; }
    public bool IsDetectEncodingFromByteOrderMarks { get; set; }
    public string? Text { get; set; }

    public UploadFilesViewModel(ILogger<UploadFilesViewModel> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task UploadSelectedFileAsync()
    {
        if (SelectedFile is null)
        {
            return;
        }

        _logger.LogDebug("File: {File}", new
        {
            SelectedFile.Name,
            SelectedFile.Size,
            SelectedFile.ContentType,
            SelectedFile.LastModified
        });

        using var multipart = new MultipartFormDataContent();
        var content = new StreamContent(SelectedFile.OpenReadStream());
        if (!string.IsNullOrEmpty(SelectedFile.ContentType))
        {
            content.Headers.ContentType = new(SelectedFile.ContentType);
        }
        multipart.Add(content, "file", SelectedFile.Name);
        var uploadTask = _httpClient.PostAsync("UploadFiles", multipart);

        (Text, EncodingCodePage) = await GetEncodingFromFileAsync(SelectedFile);

        var response = await uploadTask;
        _logger.LogDebug("HttpResponseMessage: {Response}", response);
    }

    private static async Task<(string? text, int? codepage)> GetEncodingFromFileAsync(IBrowserFile file)
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
            return (null, null);
        }
        else
        {
            return (readText, code.GetEncoding().CodePage);
        }
    }

    public async ValueTask ReadFileAsync()
    {
        if (SelectedFile is not null && EncodingCodePage.HasValue)
        {
            var encoding = Encoding.GetEncoding(EncodingCodePage.Value);
            using var reader = new StreamReader(SelectedFile.OpenReadStream(), encoding, IsDetectEncodingFromByteOrderMarks);
            Text = await reader.ReadToEndAsync();
        }
        else
        {
            Text = string.Empty;
        }
    }
}

using Notes.Blazor.Client.Services;
using Notes.Blazor.Shared;

namespace Notes.Blazor.Client.ViewModels.UploadFiles;

public class IndexViewModel
{
    private readonly IUploadFileService _uploadFileService;
    private UploadedFile[]? _uploadedFiles;

    public ReadOnlySpan<UploadedFile> UploadedFiles => _uploadedFiles;

    public IndexViewModel(IUploadFileService uploadFileService)
    {
        _uploadFileService = uploadFileService;
    }

    public async Task ListAsync()
    {
        _uploadedFiles = await _uploadFileService.ListAsync().ConfigureAwait(false);
    }
}

using Notes.Blazor.Client.Services;
using Notes.Blazor.Shared;

namespace Notes.Blazor.Client.ViewModels.UploadFiles;

public class IndexViewModel
{
    private readonly IUploadFileService _uploadFileService;
    private PagesData<UploadedFile>? _pagesData;

    public PagesInfo? PagesInfo => _pagesData?.Info;

    public IEnumerable<UploadedFile> UploadedFiles => _pagesData?.Data ?? Enumerable.Empty<UploadedFile>();

    public IndexViewModel(IUploadFileService uploadFileService)
    {
        _uploadFileService = uploadFileService;
    }

    public async Task ListAsync(int? pageNumber = null)
    {
        _pagesData = await _uploadFileService.ListAsync(pageNumber).ConfigureAwait(false);
    }
}

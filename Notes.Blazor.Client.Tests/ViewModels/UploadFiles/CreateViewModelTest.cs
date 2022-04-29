using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Notes.Blazor.Client.Services;
using Notes.Blazor.Client.ViewModels.UploadFiles;
using System.Text;
using Xunit;

namespace Notes.Blazor.Client.Tests.ViewModels.UploadFiles;

public class CreateViewModelTest
{
    [Fact]
    public async Task UploadSelectedFileAsync_SelectedFileIsNull()
    {
        var mockUploadFileService = new Mock<IUploadFileService>();
        var mockLogger = new Mock<ILogger<CreateViewModel>>();
        var mockOptions = new Mock<IOptions<NotesClientOptions>>();

        var viewModel = new CreateViewModel(mockUploadFileService.Object, mockLogger.Object, mockOptions.Object);
        await viewModel.UploadSelectedFileAsync();

        mockUploadFileService.Verify(service => service.UploadFileAsync(It.IsAny<IBrowserFile>()), Times.Never);
    }

    [Theory]
    [MemberData(nameof(UploadSelectedFileAsync_TextFile_Success_TestData))]
    public async Task UploadSelectedFileAsync_TextFile_Success(string str, Encoding encoding)
    {
        var mockUploadFileService = new Mock<IUploadFileService>();
        var mockLogger = new Mock<ILogger<CreateViewModel>>();
        var mockOptions = new Mock<IOptions<NotesClientOptions>>();
        var mockBrowserFile = new Mock<IBrowserFile>();
        mockBrowserFile.Setup(file => file.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                       .Returns(() => new MemoryStream(encoding.GetBytes(str), false));

        var viewModel = new CreateViewModel(mockUploadFileService.Object, mockLogger.Object, mockOptions.Object)
        {
            SelectedFile = mockBrowserFile.Object
        };
        await viewModel.UploadSelectedFileAsync();

        mockUploadFileService.Verify(service => service.UploadFileAsync(It.IsAny<IBrowserFile>()), Times.Once);
    }

    public static IEnumerable<object[]> UploadSelectedFileAsync_TextFile_Success_TestData()
    {
        foreach (var data in GetEncodingFromFileAsync_TextFile_Success_TestData())
        {
            yield return data.Take(2).ToArray();
        }
    }

    [Fact]
    public async Task GetEncodingFromFileAsync_SelectedFileIsNull()
    {
        var mockUploadFileService = new Mock<IUploadFileService>();
        var mockLogger = new Mock<ILogger<CreateViewModel>>();
        var mockOptions = new Mock<IOptions<NotesClientOptions>>();

        var viewModel = new CreateViewModel(mockUploadFileService.Object, mockLogger.Object, mockOptions.Object);
        await viewModel.GetEncodingFromFileAsync();

        Assert.Null(viewModel.EncodingCodePage);
        Assert.Null(viewModel.Text);
    }

    [Theory]
    [MemberData(nameof(GetEncodingFromFileAsync_TextFile_Success_TestData))]
    public async Task GetEncodingFromFileAsync_TextFile_Success(string str, Encoding encoding, int expectedCodePage)
    {
        var mockUploadFileService = new Mock<IUploadFileService>();
        var mockLogger = new Mock<ILogger<CreateViewModel>>();

        var mockOptions = new Mock<IOptions<NotesClientOptions>>();
        mockOptions.SetupGet(options => options.Value)
                   .Returns(new NotesClientOptions());

        var mockBrowserFile = new Mock<IBrowserFile>();
        mockBrowserFile.Setup(file => file.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                       .Returns(() => new MemoryStream(encoding.GetBytes(str), false));

        var viewModel = new CreateViewModel(mockUploadFileService.Object, mockLogger.Object, mockOptions.Object)
        {
            SelectedFile = mockBrowserFile.Object
        };
        await viewModel.GetEncodingFromFileAsync();

        Assert.Equal(expectedCodePage, viewModel.EncodingCodePage);
        Assert.Equal(str, viewModel.Text);
    }

    public static IEnumerable<object[]> GetEncodingFromFileAsync_TextFile_Success_TestData()
    {
        yield return new object[] { "ASCII Only", Encoding.ASCII, Encoding.UTF8.CodePage };
        yield return new object[] { "ASCII Only (UTF8 BOM)", Encoding.UTF8, Encoding.UTF8.CodePage };
        yield return new object[] { "ASCII Only (UTF8 No BOM)", new UTF8Encoding(), Encoding.UTF8.CodePage };
        yield return new object[] { "ASCII Only (SJIS)", Encoding.GetEncoding("shift_jis"), Encoding.UTF8.CodePage };
        yield return new object[] { "UTF8(BOM付き)テキスト", Encoding.UTF8, Encoding.UTF8.CodePage };
        yield return new object[] { "UTF8(BOM無し)テキスト", new UTF8Encoding(), Encoding.UTF8.CodePage };
        yield return new object[] { "SJISテキスト", Encoding.GetEncoding("shift_jis"), 932 };
    }

    [Fact]
    public async Task ReadFileAsync_SelectedFileIsNull()
    {
        var mockUploadFileService = new Mock<IUploadFileService>();
        var mockLogger = new Mock<ILogger<CreateViewModel>>();
        var mockOptions = new Mock<IOptions<NotesClientOptions>>();

        var viewModel = new CreateViewModel(mockUploadFileService.Object, mockLogger.Object, mockOptions.Object)
        {
            SelectedFile = null,
            EncodingCodePage = 932
        };
        await viewModel.ReadFileAsync();

        Assert.Null(viewModel.Text);
    }

    [Fact]
    public async Task ReadFileAsync_EncodingCodePageIsNull()
    {
        var mockUploadFileService = new Mock<IUploadFileService>();
        var mockLogger = new Mock<ILogger<CreateViewModel>>();
        var mockOptions = new Mock<IOptions<NotesClientOptions>>();
        var mockBrowserFile = new Mock<IBrowserFile>();
        mockBrowserFile.Setup(file => file.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                       .Returns(() => new MemoryStream(Encoding.UTF8.GetBytes("テストデータ"), false));

        var viewModel = new CreateViewModel(mockUploadFileService.Object, mockLogger.Object, mockOptions.Object)
        {
            SelectedFile = mockBrowserFile.Object,
            EncodingCodePage = null
        };
        await viewModel.ReadFileAsync();

        mockBrowserFile.Verify(file => file.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Null(viewModel.Text);
    }

    [Fact]
    public async Task ReadFileAsync_InvalidEncodingCodePage()
    {
        var mockUploadFileService = new Mock<IUploadFileService>();
        var mockLogger = new Mock<ILogger<CreateViewModel>>();
        var mockOptions = new Mock<IOptions<NotesClientOptions>>();
        var mockBrowserFile = new Mock<IBrowserFile>();
        mockBrowserFile.Setup(file => file.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                       .Returns(() => new MemoryStream(Encoding.UTF8.GetBytes("テストデータ"), false));

        var viewModel = new CreateViewModel(mockUploadFileService.Object, mockLogger.Object, mockOptions.Object)
        {
            SelectedFile = mockBrowserFile.Object,
            EncodingCodePage = -1
        };
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => viewModel.ReadFileAsync().AsTask());

        Assert.Null(viewModel.Text);
    }

    [Fact]
    public async Task ReadFileAsync_Success()
    {
        var mockUploadFileService = new Mock<IUploadFileService>();
        var mockLogger = new Mock<ILogger<CreateViewModel>>();

        var mockOptions = new Mock<IOptions<NotesClientOptions>>();
        mockOptions.SetupGet(options => options.Value)
                   .Returns(new NotesClientOptions());

        var mockBrowserFile = new Mock<IBrowserFile>();
        mockBrowserFile.Setup(file => file.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                       .Returns(() => new MemoryStream(Encoding.UTF8.GetBytes("テストデータ"), false));

        var viewModel = new CreateViewModel(mockUploadFileService.Object, mockLogger.Object, mockOptions.Object)
        {
            SelectedFile = mockBrowserFile.Object,
            EncodingCodePage = Encoding.UTF8.CodePage
        };
        await viewModel.ReadFileAsync();

        mockBrowserFile.Verify(file => file.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()));
        Assert.Equal("テストデータ", viewModel.Text);
    }
}

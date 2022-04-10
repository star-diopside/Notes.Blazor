using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Moq;
using Notes.Blazor.Client.Services;
using Notes.Blazor.Client.ViewModels;
using System.Text;
using Xunit;

namespace Notes.Blazor.Client.Tests.ViewModels;

public class UploadFilesViewModelTest
{
    [Fact]
    public async Task UploadSelectedFileAsync_SelectedFileIsNull()
    {
        var mockHostService = new Mock<IHostService>();
        var mockLogger = new Mock<ILogger<UploadFilesViewModel>>();

        var viewModel = new UploadFilesViewModel(mockHostService.Object, mockLogger.Object);
        await viewModel.UploadSelectedFileAsync();

        mockHostService.Verify(hostService => hostService.UploadFileAsync(It.IsAny<IBrowserFile>()), Times.Never);
    }

    [Theory]
    [MemberData(nameof(UploadSelectedFileAsync_TextFile_Success_TestData))]
    public async Task UploadSelectedFileAsync_TextFile_Success(string str, Encoding encoding)
    {
        var mockHostService = new Mock<IHostService>();
        var mockLogger = new Mock<ILogger<UploadFilesViewModel>>();
        var mockBrowserFile = new Mock<IBrowserFile>();
        mockBrowserFile.Setup(file => file.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                       .Returns(() => new MemoryStream(encoding.GetBytes(str), false));

        var viewModel = new UploadFilesViewModel(mockHostService.Object, mockLogger.Object)
        {
            SelectedFile = mockBrowserFile.Object
        };
        await viewModel.UploadSelectedFileAsync();

        mockHostService.Verify(hostService => hostService.UploadFileAsync(It.IsAny<IBrowserFile>()), Times.Once);
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
        var mockHostService = new Mock<IHostService>();
        var mockLogger = new Mock<ILogger<UploadFilesViewModel>>();

        var viewModel = new UploadFilesViewModel(mockHostService.Object, mockLogger.Object);
        await viewModel.GetEncodingFromFileAsync();

        Assert.Null(viewModel.EncodingCodePage);
        Assert.Null(viewModel.Text);
    }

    [Theory]
    [MemberData(nameof(GetEncodingFromFileAsync_TextFile_Success_TestData))]
    public async Task GetEncodingFromFileAsync_TextFile_Success(string str, Encoding encoding, int expectedCodePage)
    {
        var mockHostService = new Mock<IHostService>();
        var mockLogger = new Mock<ILogger<UploadFilesViewModel>>();
        var mockBrowserFile = new Mock<IBrowserFile>();
        mockBrowserFile.Setup(file => file.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                       .Returns(() => new MemoryStream(encoding.GetBytes(str), false));

        var viewModel = new UploadFilesViewModel(mockHostService.Object, mockLogger.Object)
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
        var mockHostService = new Mock<IHostService>();
        var mockLogger = new Mock<ILogger<UploadFilesViewModel>>();

        var viewModel = new UploadFilesViewModel(mockHostService.Object, mockLogger.Object)
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
        var mockHostService = new Mock<IHostService>();
        var mockLogger = new Mock<ILogger<UploadFilesViewModel>>();
        var mockBrowserFile = new Mock<IBrowserFile>();
        mockBrowserFile.Setup(file => file.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                       .Returns(() => new MemoryStream(Encoding.UTF8.GetBytes("テストデータ"), false));

        var viewModel = new UploadFilesViewModel(mockHostService.Object, mockLogger.Object)
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
        var mockHostService = new Mock<IHostService>();
        var mockLogger = new Mock<ILogger<UploadFilesViewModel>>();
        var mockBrowserFile = new Mock<IBrowserFile>();
        mockBrowserFile.Setup(file => file.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                       .Returns(() => new MemoryStream(Encoding.UTF8.GetBytes("テストデータ"), false));

        var viewModel = new UploadFilesViewModel(mockHostService.Object, mockLogger.Object)
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
        var mockHostService = new Mock<IHostService>();
        var mockLogger = new Mock<ILogger<UploadFilesViewModel>>();
        var mockBrowserFile = new Mock<IBrowserFile>();
        mockBrowserFile.Setup(file => file.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                       .Returns(() => new MemoryStream(Encoding.UTF8.GetBytes("テストデータ"), false));

        var viewModel = new UploadFilesViewModel(mockHostService.Object, mockLogger.Object)
        {
            SelectedFile = mockBrowserFile.Object,
            EncodingCodePage = Encoding.UTF8.CodePage
        };
        await viewModel.ReadFileAsync();

        mockBrowserFile.Verify(file => file.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>()));
        Assert.Equal("テストデータ", viewModel.Text);
    }
}

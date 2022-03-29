using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Notes.Blazor.Server.Controllers;
using Notes.Blazor.Shared;
using System.Net;
using System.Net.Mime;
using System.Text;
using Xunit;

namespace Notes.Blazor.Server.Tests.Controllers;

public class UploadFilesControllerTest
{
    [Theory]
    [MemberData(nameof(PostAsync_Success_TestData))]
    public async Task PostAsync_Success(string fileName, string contentType, long length, byte[] data)
    {
        var mockLogger = new Mock<ILogger<UploadFilesController>>();
        var mockFormFile = new Mock<IFormFile>();
        mockFormFile.SetupGet(file => file.FileName)
                    .Returns(fileName);
        mockFormFile.SetupGet(file => file.ContentType)
                    .Returns(contentType);
        mockFormFile.SetupGet(file => file.Length)
                    .Returns(length);
        mockFormFile.Setup(file => file.OpenReadStream())
                    .Returns(() => new MemoryStream(data, false));

        var controller = new UploadFilesController(mockLogger.Object);
        var result = await controller.PostAsync(mockFormFile.Object);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("Get", createdResult.ActionName);
        Assert.Null(createdResult.ControllerName);
        Assert.Equal((int)HttpStatusCode.Created, createdResult.StatusCode);
        var value = Assert.IsType<UploadFile>(createdResult.Value);
        Assert.Equal(fileName, value.FileName);
        Assert.Equal(contentType, value.ContentType);
        Assert.Equal(length, value.Length);
        Assert.NotNull(value.HashValue);
        Assert.Equal(64, value.HashValue.Length);
    }

    public static IEnumerable<object[]> PostAsync_Success_TestData()
    {
        yield return new object[] { "Empty.txt", MediaTypeNames.Text.Plain, 0, Array.Empty<byte>() };
        yield return new object[] { "ファイル.txt", MediaTypeNames.Application.Octet, 12, Encoding.UTF8.GetBytes("TestData") };
    }
}

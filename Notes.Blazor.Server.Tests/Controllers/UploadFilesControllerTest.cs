using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Notes.Blazor.Data.Models;
using Notes.Blazor.Data.Repositories;
using Notes.Blazor.Server.Controllers;
using Notes.Blazor.Shared;
using System.Net;
using System.Net.Mime;
using System.Text;
using Xunit;

namespace Notes.Blazor.Server.Tests.Controllers;

public class UploadFilesControllerTest
{
    [Fact]
    public async Task GetAsync_NotFound()
    {
        var mockUserFileRepository = new Mock<IUserFileRepository>();

        var controller = new UploadFilesController(mockUserFileRepository.Object);
        var result = await controller.GetAsync(new Random().Next());

        var notFoundResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
    }

    [Theory]
    [InlineData(1, "Empty.dat", "dummy_type_1", 0, "dummy_hash_1")]
    [InlineData(9, "ファイル.txt", "dummy_type_2", 10, "dummy_hash_2")]
    public async Task GetAsync_Success(int id, string fileName, string contentType, long length, string hashValue)
    {
        var mockUserFileRepository = new Mock<IUserFileRepository>();
        mockUserFileRepository.Setup(repository => repository.FindByIdAsync(id))
                              .ReturnsAsync(() => new UserFile(fileName, contentType, length, hashValue) { Id = id });

        var controller = new UploadFilesController(mockUserFileRepository.Object);
        var result = await controller.GetAsync(id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        var uploadFile = Assert.IsType<UploadFile>(okResult.Value);
        Assert.Equal(fileName, uploadFile.FileName);
        Assert.Equal(contentType, uploadFile.ContentType);
        Assert.Equal(length, uploadFile.Length);
        Assert.Equal(hashValue, uploadFile.HashValue);
    }

    [Theory]
    [MemberData(nameof(PostAsync_Success_TestData))]
    public async Task PostAsync_Success(string fileName, string contentType, long length, byte[] data, int id)
    {
        var mockUserFileRepository = new Mock<IUserFileRepository>();
        mockUserFileRepository.Setup(repository => repository.Add(It.IsAny<UserFile>()))
                              .Returns((UserFile userFile) => new UserFile(
                                  fileName: userFile.FileName,
                                  contentType: userFile.ContentType,
                                  length: userFile.Length,
                                  hashValue: userFile.HashValue)
                              {
                                  Id = id
                              });

        var mockFormFile = new Mock<IFormFile>();
        mockFormFile.SetupGet(file => file.FileName)
                    .Returns(fileName);
        mockFormFile.SetupGet(file => file.ContentType)
                    .Returns(contentType);
        mockFormFile.SetupGet(file => file.Length)
                    .Returns(length);
        mockFormFile.Setup(file => file.OpenReadStream())
                    .Returns(() => new MemoryStream(data, false));

        var controller = new UploadFilesController(mockUserFileRepository.Object);
        var result = await controller.PostAsync(mockFormFile.Object);

        mockUserFileRepository.Verify(repository => repository.Add(It.Is<UserFile>(
            userFile => userFile.FileName == fileName
                        && userFile.ContentType == contentType
                        && userFile.Length == length
                        && userFile.HashValue.Length == 64)));
        mockUserFileRepository.Verify(repository => repository.SaveChangesAsync());
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("Get", createdResult.ActionName);
        Assert.Null(createdResult.ControllerName);
        Assert.Equal(id, createdResult.RouteValues?["id"]);
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
        var random = new Random();
        yield return new object[] { "Empty.txt", MediaTypeNames.Text.Plain, 0, Array.Empty<byte>(), random.Next() };
        yield return new object[] { "ファイル.txt", MediaTypeNames.Application.Octet, 12, Encoding.UTF8.GetBytes("TestData"), random.Next() };
    }
}

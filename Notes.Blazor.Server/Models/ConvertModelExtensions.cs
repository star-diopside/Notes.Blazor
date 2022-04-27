using Notes.Blazor.Data.Models;
using Notes.Blazor.Shared;

namespace Notes.Blazor.Server.Models;

public static class ConvertModelExtensions
{
    /// <summary>
    /// <see cref="UserFile"/>オブジェクトを<see cref="UploadedFile"/>に変換する。
    /// </summary>
    /// <param name="userFile">変換元の<see cref="UserFile"/>オブジェクト</param>
    /// <returns>変換した<see cref="UploadedFile"/>オブジェクト</returns>
    public static UploadedFile ToUploadedFile(this UserFile userFile)
    {
        return new UploadedFile(
            Id: userFile.Id,
            FileName: userFile.FileName,
            ContentType: userFile.ContentType,
            Length: userFile.Length,
            HashValue: userFile.HashValue);
    }
}

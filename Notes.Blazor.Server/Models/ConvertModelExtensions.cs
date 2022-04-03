using Notes.Blazor.Data.Models;
using Notes.Blazor.Shared;

namespace Notes.Blazor.Server.Models;

public static class ConvertModelExtensions
{
    /// <summary>
    /// <see cref="UserFile"/>オブジェクトを<see cref="UploadFile"/>に変換する。
    /// </summary>
    /// <param name="userFile">変換元の<see cref="UserFile"/>オブジェクト</param>
    /// <returns>変換した<see cref="UploadFile"/>オブジェクト</returns>
    public static UploadFile ToUploadFile(this UserFile userFile)
    {
        return new UploadFile(userFile.FileName, userFile.ContentType, userFile.Length, userFile.HashValue);
    }
}

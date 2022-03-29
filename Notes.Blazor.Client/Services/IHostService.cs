using Microsoft.AspNetCore.Components.Forms;
using Notes.Blazor.Shared;

namespace Notes.Blazor.Client.Services;

public interface IHostService
{
    /// <summary>
    /// ファイルをアップロードする。
    /// </summary>
    /// <param name="file">アップロードするファイル</param>
    /// <returns>非同期操作を表すタスクオブジェクト</returns>
    Task<UploadFile?> UploadFileAsync(IBrowserFile file);
}

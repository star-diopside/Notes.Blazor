using Microsoft.AspNetCore.Components.Forms;
using Notes.Blazor.Shared;

namespace Notes.Blazor.Client.Services;

public interface IUploadFileService
{
    /// <summary>
    /// アップロードされたファイル一覧を取得する。
    /// </summary>
    /// <returns>非同期操作を表すタスクオブジェクト</returns>
    Task<UploadedFile[]?> ListAsync();

    /// <summary>
    /// ファイルをアップロードする。
    /// </summary>
    /// <param name="file">アップロードするファイル</param>
    /// <returns>非同期操作を表すタスクオブジェクト</returns>
    Task<UploadedFile?> UploadFileAsync(IBrowserFile file);
}

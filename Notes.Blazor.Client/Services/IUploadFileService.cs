using Microsoft.AspNetCore.Components.Forms;
using Notes.Blazor.Shared;

namespace Notes.Blazor.Client.Services;

public interface IUploadFileService
{
    /// <summary>
    /// アップロードされたファイル一覧を取得する。
    /// </summary>
    /// <param name="pageNumber">データを取得するページ番号。<c>null</c>の場合は最初のページを取得する。</param>
    /// <returns>非同期操作を表すタスクオブジェクト</returns>
    Task<PagesData<UploadedFile>?> ListAsync(int? pageNumber = null);

    /// <summary>
    /// ファイルをアップロードする。
    /// </summary>
    /// <param name="file">アップロードするファイル</param>
    /// <returns>非同期操作を表すタスクオブジェクト</returns>
    Task<UploadedFile?> UploadFileAsync(IBrowserFile file);
}

namespace Notes.Blazor.Client;

public class NotesClientOptions
{
    /// <summary>
    /// アップロード可能なファイルサイズ
    /// </summary>
    public long UploadAllowedSize { get; set; } = 500 * 1024;

    /// <summary>
    /// 一覧画面で表示する1ページ当たりの件数
    /// </summary>
    public int? PageSize { get; set; }
}

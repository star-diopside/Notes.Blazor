namespace Notes.Blazor.Client;

public class NotesClientOptions
{
    /// <summary>
    /// アップロード可能なファイルサイズ
    /// </summary>
    public long UploadAllowedSize { get; set; } = 500 * 1024;
}

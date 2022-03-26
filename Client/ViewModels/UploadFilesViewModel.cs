using Hnx8.ReadJEnc;
using Microsoft.AspNetCore.Components.Forms;
using Notes.Blazor.Client.Services;
using System.Text;

namespace Notes.Blazor.Client.ViewModels;

public class UploadFilesViewModel
{
    private readonly IHostService _hostService;
    private readonly ILogger<UploadFilesViewModel> _logger;

    /// <summary>選択されたファイル</summary>
    public IBrowserFile? SelectedFile { get; set; }

    /// <summary>使用可能な文字エンコード一覧</summary>
    public IEnumerable<EncodingInfo> Encodings { get; } = Encoding.GetEncodings().OrderBy(info => info.CodePage).ToArray();

    /// <summary>テキストを読み取る文字エンコードのコードページ</summary>
    public int? EncodingCodePage { get; set; }

    /// <summary>
    /// ファイルの先頭にあるバイト順序マーク(BOM)を検索するかどうかを示す値<br/>
    /// テキストを読み取る際にBOMを優先する場合は<c>true</c>、<see cref="EncodingCodePage"/>で指定された文字エンコードを優先する場合は<c>false</c>。
    /// </summary>
    public bool IsDetectEncodingFromByteOrderMarks { get; set; }

    /// <summary>テキストファイルから取得した文字列</summary>
    public string? Text { get; set; }

    public UploadFilesViewModel(IHostService hostService, ILogger<UploadFilesViewModel> logger)
    {
        _hostService = hostService;
        _logger = logger;
    }

    /// <summary>
    /// <see cref="SelectedFile"/>をアップロードする。
    /// </summary>
    /// <returns>非同期操作を表すタスクオブジェクト</returns>
    public async Task UploadSelectedFileAsync()
    {
        if (SelectedFile is null)
        {
            return;
        }

        _logger.LogInformation("SelectedFile: {SelectedFile}", new
        {
            SelectedFile.Name,
            SelectedFile.Size,
            SelectedFile.ContentType,
            SelectedFile.LastModified
        });

        var uploadTask = _hostService.UploadFileAsync(SelectedFile);

        (EncodingCodePage, Text) = await GetEncodingFromFileAsync(SelectedFile);

        var result = await uploadTask;
        _logger.LogInformation("Upload Result: {Result}", result);
    }

    /// <summary>
    /// ファイルから文字エンコードを自動判別する。
    /// </summary>
    /// <param name="file">ファイル</param>
    /// <returns>
    /// 非同期操作を表すタスクオブジェクト<br/>
    /// 処理結果としてファイルの内容から自動判別した文字エンコードのコードページと取得した文字列を返す。<br/>
    /// バイナリファイルの場合は<c>(null, null)</c>を返す。
    /// </returns>
    private static async Task<(int? codepage, string? text)> GetEncodingFromFileAsync(IBrowserFile file)
    {
        byte[] array;

        using (var stream = file.OpenReadStream())
        using (var memory = new MemoryStream())
        {
            await stream.CopyToAsync(memory);
            array = memory.ToArray();
        }

        var code = ReadJEnc.JP.GetEncoding(array, array.Length, out var readText);
        return (code?.GetEncoding().CodePage, readText);
    }

    /// <summary>
    /// ファイルからテキストを読み取る。<br/>
    /// <see cref="SelectedFile"/>から<see cref="EncodingCodePage"/>コードページの文字エンコードでテキストを読み取り、<see cref="Text"/>に設定する。<br/>
    /// BOMの解釈は<see cref="IsDetectEncodingFromByteOrderMarks"/>の設定に準ずる。
    /// </summary>
    /// <returns>非同期操作を表すタスクオブジェクト</returns>
    public async ValueTask ReadFileAsync()
    {
        if (SelectedFile is not null && EncodingCodePage.HasValue)
        {
            var encoding = Encoding.GetEncoding(EncodingCodePage.Value);
            using var reader = new StreamReader(SelectedFile.OpenReadStream(), encoding, IsDetectEncodingFromByteOrderMarks);
            Text = await reader.ReadToEndAsync();
        }
        else
        {
            Text = null;
        }
    }
}

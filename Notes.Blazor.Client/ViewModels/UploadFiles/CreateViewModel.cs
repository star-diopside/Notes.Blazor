using ColorCode;
using Hnx8.ReadJEnc;
using Markdig;
using Markdown.ColorCode;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Notes.Blazor.Client.Services;
using System.Text;

namespace Notes.Blazor.Client.ViewModels.UploadFiles;

public enum TextType { Plain, Markdown, Language }

public class CreateViewModel
{
    private readonly IUploadFileService _uploadFileService;
    private readonly ILogger<CreateViewModel> _logger;
    private readonly MarkdownPipeline _markdownPipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseColorCode()
        .Build();
    private TextType _textType;
    private string? _languageId;
    private string? _text;
    private MarkupString? _markupString;

    /// <summary>選択されたファイル</summary>
    public IBrowserFile? SelectedFile { get; set; }

    /// <summary>使用可能な文字エンコード一覧</summary>
    public IEnumerable<EncodingInfo> SupportedEncodings => Encoding.GetEncodings().OrderBy(info => info.CodePage);

    /// <summary>テキストを読み取る文字エンコードのコードページ</summary>
    public int? EncodingCodePage { get; set; }

    /// <summary>
    /// ファイルの先頭にあるバイト順序マーク(BOM)を検索するかどうかを示す値<br/>
    /// テキストを読み取る際にBOMを優先する場合は<c>true</c>、<see cref="EncodingCodePage"/>で指定された文字エンコードを優先する場合は<c>false</c>。
    /// </summary>
    public bool IsDetectEncodingFromByteOrderMarks { get; set; }

    /// <summary>表示するテキストタイプ</summary>
    public TextType TextType
    {
        get => _textType;
        set
        {
            _textType = value;
            _markupString = null;
        }
    }

    /// <summary>マークアップをサポートする言語一覧</summary>
    public IEnumerable<ILanguage> SupportedLanguages => Languages.All.OrderBy(lang => lang.Name);

    /// <summary>マークアップする言語ID</summary>
    public string? LanguageId
    {
        get => _languageId;
        set
        {
            _languageId = value;
            _markupString = null;
        }
    }

    /// <summary>テキストファイルから取得した文字列</summary>
    public string? Text
    {
        get => _text;
        set
        {
            _text = value;
            _markupString = null;
        }
    }

    /// <summary>
    /// <see cref="Text"/>をマークアップした文字列
    /// </summary>
    public MarkupString MarkupString => _markupString ??= Text is null ? default : (MarkupString)(TextType switch
    {
        TextType.Markdown => Markdig.Markdown.ToHtml(Text, _markdownPipeline),
        TextType.Language => GetHtmlString(),
        _ => throw new InvalidOperationException()
    });

    private string GetHtmlString()
    {
        var language = string.IsNullOrEmpty(LanguageId) ? null : Languages.FindById(LanguageId);
        return language is null ? string.Empty : new HtmlFormatter().GetHtmlString(Text, language);
    }

    public CreateViewModel(IUploadFileService uploadFileService, ILogger<CreateViewModel> logger)
    {
        _uploadFileService = uploadFileService;
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

        var result = await _uploadFileService.UploadFileAsync(SelectedFile).ConfigureAwait(false);

        _logger.LogInformation("Upload Result: {Result}", result);
    }

    /// <summary>
    /// <see cref="SelectedFile"/>から文字エンコードを自動判別する。
    /// </summary>
    /// <returns>非同期操作を表すタスクオブジェクト</returns>
    public async Task GetEncodingFromFileAsync()
    {
        if (SelectedFile is not null)
        {
            (EncodingCodePage, Text) = await GetEncodingFromFileAsync(SelectedFile).ConfigureAwait(false);
        }
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
            await stream.CopyToAsync(memory).ConfigureAwait(false);
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
            var encoding = Encoding.GetEncoding(EncodingCodePage.GetValueOrDefault());
            using var reader = new StreamReader(SelectedFile.OpenReadStream(), encoding, IsDetectEncodingFromByteOrderMarks);
            Text = await reader.ReadToEndAsync().ConfigureAwait(false);
        }
        else
        {
            Text = null;
        }
    }
}

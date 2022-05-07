using Notes.Blazor.Data.Models;
using Notes.Blazor.Shared;
using X.PagedList;

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

    /// <summary>
    /// <see cref="IPagedList"/>オブジェクトを<see cref="PagesInfo"/>に変換する。
    /// </summary>
    /// <param name="pagedList">変換元の<see cref="IPagedList"/>オブジェクト</param>
    /// <returns>変換した<see cref="PagesInfo"/>オブジェクト</returns>
    public static PagesInfo ToPagesInfo(this IPagedList pagedList)
    {
        return new PagesInfo(
            PageCount: pagedList.PageCount,
            TotalItemCount: pagedList.TotalItemCount,
            PageNumber: pagedList.PageNumber,
            PageSize: pagedList.PageSize,
            HasPreviousPage: pagedList.HasPreviousPage,
            HasNextPage: pagedList.HasNextPage,
            IsFirstPage: pagedList.IsFirstPage,
            IsLastPage: pagedList.IsLastPage,
            FirstItemOnPage: pagedList.FirstItemOnPage,
            LastItemOnPage: pagedList.LastItemOnPage);
    }

    /// <summary>
    /// <see cref="IPagedList{T}"/>オブジェクトを<see cref="PagesData{T}"/>に変換する。
    /// </summary>
    /// <param name="pagedList">変換元の<see cref="IPagedList{T}"/>オブジェクト</param>
    /// <returns>変換した<see cref="PagesData{T}"/>オブジェクト</returns>
    public static PagesData<T> ToPagesData<T>(this IPagedList<T> pagedList)
    {
        return new PagesData<T>(pagedList)
        {
            Info = pagedList.ToPagesInfo()
        };
    }
}

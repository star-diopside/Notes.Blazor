namespace Notes.Blazor.Shared;

public record PagesInfo(
    int PageCount,
    int TotalItemCount,
    int PageNumber,
    int PageSize,
    bool HasPreviousPage,
    bool HasNextPage,
    bool IsFirstPage,
    bool IsLastPage,
    int FirstItemOnPage,
    int LastItemOnPage);

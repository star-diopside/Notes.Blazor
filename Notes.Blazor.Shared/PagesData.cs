namespace Notes.Blazor.Shared;

public record PagesData<T>(IEnumerable<T> Data)
{
    public PagesInfo? Info { get; init; }
}

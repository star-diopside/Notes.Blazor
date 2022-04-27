namespace Notes.Blazor.Shared;

public record UploadedFile(int Id, string FileName, string? ContentType, long Length, string HashValue);

namespace Notes.Blazor.Shared;

public record UploadFile(string FileName, string? ContentType, long Length, string HashValue);

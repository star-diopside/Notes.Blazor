using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notes.Blazor.Data.Models;

public class UserFile : ITrackable
{
    public int Id { get; set; }

    public string FileName { get; set; }

    public string? ContentType { get; set; }

    public long Length { get; set; }

    [MaxLength(64)]
    public string HashValue { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [Timestamp]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column("xmin", TypeName = "xid")]
    public uint Version { get; set; }

    public UserFile(string fileName, long length, string hashValue)
    {
        FileName = fileName;
        Length = length;
        HashValue = hashValue;
    }
}

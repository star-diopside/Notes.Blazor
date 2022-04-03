using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notes.Blazor.Data.Models;

public class UserFile
{
    public int Id { get; set; }

    [Required]
    public string FileName { get; set; }

    [Required]
    public string ContentType { get; set; }

    [Required]
    public long Length { get; set; }

    [Required]
    public string HashValue { get; set; }

    [Timestamp]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column("xmin", TypeName = "xid")]
    public uint Version { get; set; }

    public UserFile(string fileName, string contentType, long length, string hashValue)
    {
        FileName = fileName;
        ContentType = contentType;
        Length = length;
        HashValue = hashValue;
    }
}

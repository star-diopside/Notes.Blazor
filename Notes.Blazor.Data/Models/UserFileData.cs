using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notes.Blazor.Data.Models;

public class UserFileData : ITrackable
{
    private UserFile? _userFile;

    public int Id { get; set; }

    public int UserFileId { get; set; }

    public byte[]? Data { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [Timestamp]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column("xmin", TypeName = "xid")]
    public uint Version { get; set; }

    [BackingField(nameof(_userFile))]
    public UserFile UserFile
    {
        get => _userFile ?? throw new InvalidOperationException();
        set => _userFile = value;
    }
}

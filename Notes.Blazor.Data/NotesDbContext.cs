using Microsoft.EntityFrameworkCore;
using Notes.Blazor.Data.Models;

namespace Notes.Blazor.Data;

public class NotesDbContext : DbContext
{
    public DbSet<UserFile> UserFiles => Set<UserFile>();

    public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
}

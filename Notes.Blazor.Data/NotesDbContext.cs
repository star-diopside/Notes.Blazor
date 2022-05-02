using Microsoft.EntityFrameworkCore;
using Notes.Blazor.Data.Models;

namespace Notes.Blazor.Data;

public class NotesDbContext : DbContext
{
    public DbSet<UserFile> UserFiles => Set<UserFile>();
    public DbSet<UserFileData> UserFileData => Set<UserFileData>();

    public NotesDbContext(DbContextOptions<NotesDbContext> options) : base(options)
    {
        SavingChanges += OnSavingChanges;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }

    private void OnSavingChanges(object? sender, SavingChangesEventArgs e)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<ITrackable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    var entity = entry.Entity;
                    entity.CreatedAt = now;
                    entity.UpdatedAt = now;
                    break;

                case EntityState.Modified:
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    entry.Entity.UpdatedAt = now;
                    break;
            }
        }
    }
}

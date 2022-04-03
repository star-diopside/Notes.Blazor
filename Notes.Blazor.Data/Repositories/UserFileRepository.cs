using Notes.Blazor.Data.Models;

namespace Notes.Blazor.Data.Repositories;

public class UserFileRepository : IUserFileRepository
{
    private readonly NotesDbContext _context;

    public UserFileRepository(NotesDbContext context)
    {
        _context = context;
    }

    public ValueTask<UserFile?> FindByIdAsync(int id)
    {
        return _context.UserFiles.FindAsync(id);
    }

    public UserFile Add(UserFile userFile)
    {
        return _context.UserFiles.Add(userFile).Entity;
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using Notes.Blazor.Data.Models;
using System.Linq.Expressions;
using X.PagedList;

namespace Notes.Blazor.Data.Repositories;

public class UserFileRepository : IUserFileRepository
{
    private readonly NotesDbContext _context;

    public UserFileRepository(NotesDbContext context)
    {
        _context = context;
    }

    public IAsyncEnumerable<TResult> ListAsync<TResult>(Expression<Func<UserFile, TResult>> selector)
    {
        return _context.UserFiles.OrderBy(file => file.Id)
                                 .Select(selector)
                                 .AsAsyncEnumerable();
    }

    public Task<IPagedList<TResult>> ListAsync<TResult>(Expression<Func<UserFile, TResult>> selector, int pageNumber, int pageSize)
    {
        return _context.UserFiles.OrderBy(file => file.Id)
                                 .Select(selector)
                                 .ToPagedListAsync(pageNumber, pageSize);
    }

    public ValueTask<UserFile?> FindByIdAsync(int id)
    {
        return _context.UserFiles.FindAsync(id);
    }

    public Task<UserFile?> GetUserFileDataAsync(int id)
    {
        return _context.UserFiles.Include(file => file.UserFileData)
                                 .Where(file => file.Id == id)
                                 .SingleOrDefaultAsync();
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

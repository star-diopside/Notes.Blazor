using Notes.Blazor.Data.Models;
using System.Linq.Expressions;
using X.PagedList;

namespace Notes.Blazor.Data.Repositories;

public interface IUserFileRepository
{
    IAsyncEnumerable<TResult> ListAsync<TResult>(Expression<Func<UserFile, TResult>> selector);

    Task<IPagedList<TResult>> ListAsync<TResult>(Expression<Func<UserFile, TResult>> selector, int pageNumber, int pageSize);

    ValueTask<UserFile?> FindByIdAsync(int id);

    Task<UserFile?> GetUserFileDataAsync(int id);

    UserFile Add(UserFile userFile);

    Task<int> SaveChangesAsync();
}

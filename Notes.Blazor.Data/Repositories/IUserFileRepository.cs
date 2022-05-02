using Notes.Blazor.Data.Models;
using System.Linq.Expressions;

namespace Notes.Blazor.Data.Repositories;

public interface IUserFileRepository
{
    IAsyncEnumerable<TResult> FindAllAsync<TResult>(Expression<Func<UserFile, TResult>> selector);

    ValueTask<UserFile?> FindByIdAsync(int id);

    Task<UserFile?> GetUserFileDataAsync(int id);

    UserFile Add(UserFile userFile);

    Task<int> SaveChangesAsync();
}

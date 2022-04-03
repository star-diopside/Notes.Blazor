using Notes.Blazor.Data.Models;

namespace Notes.Blazor.Data.Repositories;

public interface IUserFileRepository
{
    ValueTask<UserFile?> FindByIdAsync(int id);

    UserFile Add(UserFile userFile);

    Task<int> SaveChangesAsync();
}

using Service.Identity.Models;

namespace Service.Identity.Abstractions;

public interface IUserRepository
{
    public Task<UserDatabaseModel> CreateAsync(UserDatabaseModel userDatabaseModel);
    public Task<UserDatabaseModel?> FindByEmail(string email);
}
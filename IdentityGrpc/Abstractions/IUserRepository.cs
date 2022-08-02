using IdentityGrpc.Models;

namespace IdentityGrpc.Abstractions;

public interface IUserRepository
{
    public Task<UserDatabaseModel> CreateAsync(UserDatabaseModel userDatabaseModel);
    public Task<UserDatabaseModel?> FindByEmail(string email);
}
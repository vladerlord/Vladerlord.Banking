using Service.Identity.Models;

namespace Service.Identity.Abstractions;

public interface IUserRepository
{
	public Task<UserDatabaseModel> CreateAsync(UserDatabaseModel userDatabaseModel);
	public Task<UserDatabaseModel?> FindByIdAsync(Guid id);
	public Task<UserDatabaseModel?> FindByEmailAsync(string email);
	public Task UpdateStatusAsync(Guid id, UserStatus status);
}
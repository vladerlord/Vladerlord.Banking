using Service.Identity.Models;

namespace Service.Identity.Abstractions;

public interface IConfirmationLinkRepository
{
	public Task<ConfirmationLinkDatabaseModel> CreateAsync(ConfirmationLinkDatabaseModel model);
	public Task<ConfirmationLinkDatabaseModel?> FindByConfirmationCodeAsync(string confirmationCode);
	public Task DeleteByIdAsync(Guid id);
}
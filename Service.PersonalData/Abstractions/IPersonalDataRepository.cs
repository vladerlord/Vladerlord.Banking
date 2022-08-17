using Service.PersonalData.Models;

namespace Service.PersonalData.Abstractions;

public interface IPersonalDataRepository
{
	public Task<PersonalDataDatabaseModel> CreateOrUpdateAsync(PersonalDataDatabaseModel model);
	public Task<PersonalDataDatabaseModel?> FindByUserIdAsync(Guid userId);
	public Task<PersonalDataDatabaseModel?> FindByIdAsync(Guid personalDataId);
	public Task<List<PersonalDataDatabaseModel>> GetUnapprovedAsync();
	public Task ChangeStatusAsync(Guid id, PersonalDataStatus status);
}
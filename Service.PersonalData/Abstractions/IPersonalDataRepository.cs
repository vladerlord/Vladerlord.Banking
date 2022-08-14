using Service.PersonalData.Models;

namespace Service.PersonalData.Abstractions;

public interface IPersonalDataRepository
{
	public Task<PersonalDataDatabaseModel> CreateOrUpdateAsync(PersonalDataDatabaseModel model);
}
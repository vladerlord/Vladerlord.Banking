using Service.PersonalData.Models;
using Shared.Grpc.PersonalData.Contracts;

namespace Service.PersonalData.Abstractions;

public interface IPersonalDataService
{
    Task<PersonalDataDatabaseModel?> FindByUserId(Guid userId);
    Task<PersonalDataDatabaseModel> ApplyPersonalDataAsync(ApplyPersonalDataGrpcRequest request);
    Task<IEnumerable<PersonalDataDatabaseModel>> GetUnapprovedAsync();
    Task<PersonalDataDatabaseModel> GetByIdAsync(Guid personalDataId);
    Task<bool> Approve(PersonalDataDatabaseModel model);
    Task<bool> Decline(PersonalDataDatabaseModel model);
}

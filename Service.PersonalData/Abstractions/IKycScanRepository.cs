using Service.PersonalData.Models;

namespace Service.PersonalData.Abstractions;

public interface IKycScanRepository
{
    public Task<KycScanDatabaseModel> CreateAsync(KycScanDatabaseModel model);
    public Task<IEnumerable<KycScanDatabaseModel>> GetByPersonalDataIdAsync(Guid personalDataId);
    public Task<KycScanDatabaseModel?> FindByIdAsync(Guid id);
    public Task DeleteByPersonalDataIdAsync(Guid personalDataId);
}

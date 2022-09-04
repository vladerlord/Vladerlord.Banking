using Service.PersonalData.Models;
using Shared.Grpc.PersonalData.Models;

namespace Service.PersonalData.Abstractions;

public interface IKycScansService
{
    Task Create(List<KycScanCreateGrpcModel> kycScans, PersonalDataDatabaseModel personalData);
    Task DeleteAllByPersonalDataId(Guid personalDataId);
    Task<KycScanGrpcModel?> FindById(Guid id);
    Task<IEnumerable<KycScanGrpcModel>> GetAllByPersonalData(PersonalDataDatabaseModel personalData);
}

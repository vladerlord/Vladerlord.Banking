using Service.PersonalData.Models;
using Shared.Grpc.PersonalData.Models;

namespace Service.PersonalData.Abstractions;

public interface IKycScansFileExplorer
{
    Task SaveKycScan(KycScanCreateGrpcModel kycScanGrpcModel, string fileName);
    void DeleteKycScan(KycScanDatabaseModel kycScan);
    Task<byte[]> GetKyсScanContentAsync(KycScanDatabaseModel kycScan);
}

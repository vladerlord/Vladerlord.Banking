using Service.PersonalData.Abstractions;
using Service.PersonalData.Exceptions;
using Service.PersonalData.Models;
using Shared.Grpc.PersonalData.Models;

namespace Service.PersonalData.Services;

public class KycScansService : IKycScansService
{
    private readonly IKycScansFileExplorer _kycScansFileExplorer;
    private readonly IKycScanRepository _kycScanRepository;
    private readonly KycScanEncryptionService _kycScanEncryptionService;

    public KycScansService(
        IKycScansFileExplorer kycScansFileExplorer,
        IKycScanRepository kycScanRepository,
        KycScanEncryptionService kycScanEncryptionService
    )
    {
        _kycScansFileExplorer = kycScansFileExplorer;
        _kycScanRepository = kycScanRepository;

        _kycScanEncryptionService = kycScanEncryptionService;
    }

    public async Task Create(List<KycScanCreateGrpcModel> kycScans, PersonalDataDatabaseModel personalData)
    {
        foreach (var kycScanCreateModel in kycScans)
        {
            var kycScanDatabaseModel = kycScanCreateModel.ToDatabaseModel(personalData.Id);

            _kycScanEncryptionService.EncryptCreateGrpcModel(kycScanCreateModel, personalData.Iv);

            await _kycScansFileExplorer.SaveKycScan(kycScanCreateModel, kycScanDatabaseModel.FileName.ToString());
            await _kycScanRepository.CreateAsync(kycScanDatabaseModel);
        }
    }

    public async Task DeleteAllByPersonalDataId(Guid personalDataId)
    {
        var kycScans = await _kycScanRepository.GetByPersonalDataIdAsync(personalDataId);

        foreach (var kycScanDatabaseModel in kycScans)
            _kycScansFileExplorer.DeleteKycScan(kycScanDatabaseModel);

        await _kycScanRepository.DeleteByPersonalDataIdAsync(personalDataId);
    }

    public async Task<KycScanGrpcModel?> FindById(Guid id)
    {
        var kycScan = await _kycScanRepository.FindByIdAsync(id);

        if (kycScan == null)
            return null;

        var content = await _kycScansFileExplorer.GetKyсScanContentAsync(kycScan);
        var result = kycScan.ToGrpcModel(content);

        try
        {
            await _kycScanEncryptionService.DecryptGrpcModel(result);
        }
        catch (PersonalDataNotFoundException)
        {
            return null;
        }

        return result;
    }

    public async Task<IEnumerable<KycScanGrpcModel>> GetAllByPersonalData(PersonalDataDatabaseModel personalData)
    {
        var kycScans = await _kycScanRepository.GetByPersonalDataIdAsync(personalData.Id);
        var result = new List<KycScanGrpcModel>();

        foreach (var kycScanDatabaseModel in kycScans)
        {
            var content = await _kycScansFileExplorer.GetKyсScanContentAsync(kycScanDatabaseModel);
            var grpcModel = kycScanDatabaseModel.ToGrpcModel(content);

            _kycScanEncryptionService.DecryptGrpcModel(grpcModel, personalData.Iv);

            result.Add(grpcModel);
        }

        return result;
    }
}

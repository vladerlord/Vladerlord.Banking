using Service.PersonalData.Abstractions;
using Service.PersonalData.Models;
using Shared.Abstractions.Grpc.PersonalData.Models;

namespace Service.PersonalData.Services;

public class KycScansService
{
	private readonly KycScansFileService _kycScansFileService;
	private readonly IKycScanRepository _kycScanRepository;
	private readonly KycScanEncryptionService _kycScanEncryptionService;

	public KycScansService(KycScansFileService kycScansFileService, IKycScanRepository kycScanRepository,
		KycScanEncryptionService kycScanEncryptionService)
	{
		_kycScansFileService = kycScansFileService;
		_kycScanRepository = kycScanRepository;
		_kycScanEncryptionService = kycScanEncryptionService;
	}

	public async Task Create(List<KycScanGrpcModel> kycScans, PersonalDataDatabaseModel personalData)
	{
		foreach (var kycScanGrpcModel in kycScans)
		{
			var kycScanDatabaseModel = kycScanGrpcModel.ToDatabaseModel(personalData.Id);

			_kycScanEncryptionService.EncryptGrpcModel(kycScanGrpcModel, personalData.Iv);

			await _kycScansFileService.SaveKycScan(kycScanGrpcModel, kycScanDatabaseModel.FileName.ToString());
			await _kycScanRepository.CreateAsync(kycScanDatabaseModel);
		}
	}

	public async Task DeleteAllByPersonalDataId(Guid personalDataId)
	{
		var kycScans = await _kycScanRepository.GetByPersonalDataIdAsync(personalDataId);

		foreach (var kycScanDatabaseModel in kycScans)
			_kycScansFileService.DeleteKycScan(kycScanDatabaseModel);

		await _kycScanRepository.DeleteByPersonalDataIdAsync(personalDataId);
	}
}
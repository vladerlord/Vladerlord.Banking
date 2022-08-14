using Service.PersonalData.Abstractions;
using Service.PersonalData.Models;
using Shared.Abstractions.Grpc.PersonalData.Contracts;

namespace Service.PersonalData.Services;

public class PersonalDataService
{
	private readonly IPersonalDataRepository _personalDataRepository;
	private readonly KycScansService _kycScansService;
	private readonly EncryptionService _encryptionService;

	public PersonalDataService(IPersonalDataRepository personalDataRepository, KycScansService kycScansService,
		EncryptionService encryptionService)
	{
		_personalDataRepository = personalDataRepository;
		_kycScansService = kycScansService;
		_encryptionService = encryptionService;
	}

	public async Task ApplyPersonalDataAsync(ApplyPersonalDataGrpcRequest request)
	{
		var iv = _encryptionService.GenerateIv();
		var personalData = await _personalDataRepository.CreateOrUpdateAsync(request.ToPersonalDataDatabaseModel(iv));

		await _kycScansService.DeleteAllByPersonalDataId(personalData.Id);
		await _kycScansService.Create(request.KycScans, personalData);
	}
}

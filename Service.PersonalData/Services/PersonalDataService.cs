using Service.PersonalData.Abstractions;
using Service.PersonalData.Models;
using Shared.Abstractions.Grpc.PersonalData.Contracts;

namespace Service.PersonalData.Services;

public class PersonalDataService
{
	private readonly IPersonalDataRepository _personalDataRepository;
	private readonly KycScansService _kycScansService;
	private readonly EncryptionService _encryptionService;
	private readonly PersonalDataEncryptionService _personalDataEncryptionService;

	public PersonalDataService(IPersonalDataRepository personalDataRepository, KycScansService kycScansService,
		EncryptionService encryptionService, PersonalDataEncryptionService personalDataEncryptionService)
	{
		_personalDataRepository = personalDataRepository;
		_kycScansService = kycScansService;
		_encryptionService = encryptionService;
		_personalDataEncryptionService = personalDataEncryptionService;
	}

	public async Task<PersonalDataDatabaseModel> ApplyPersonalDataAsync(ApplyPersonalDataGrpcRequest request)
	{
		var iv = _encryptionService.GenerateIv();
		var databasedModel = request.ToPersonalDataDatabaseModel(iv);

		_personalDataEncryptionService.Encrypt(databasedModel);

		var personalData = await _personalDataRepository.CreateOrUpdateAsync(databasedModel);

		await _kycScansService.DeleteAllByPersonalDataId(personalData.Id);
		await _kycScansService.Create(request.KycScans, personalData);

		_personalDataEncryptionService.Decrypt(personalData);

		return personalData;
	}
}

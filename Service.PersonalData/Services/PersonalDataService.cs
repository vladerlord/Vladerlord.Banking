using Service.PersonalData.Abstractions;
using Service.PersonalData.Exceptions;
using Service.PersonalData.Models;
using Shared.Grpc.PersonalData.Contracts;

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

	public async Task<PersonalDataDatabaseModel?> FindByUserId(Guid userId)
	{
		var personalData = await _personalDataRepository.FindByUserIdAsync(userId);

		if (personalData != null)
			_personalDataEncryptionService.Decrypt(personalData);

		return personalData;
	}

	public async Task<PersonalDataDatabaseModel> ApplyPersonalDataAsync(ApplyPersonalDataGrpcRequest request)
	{
		var iv = _encryptionService.GenerateIv();
		var databasedModel = request.ToPersonalDataDatabaseModel(iv, PersonalDataStatus.PendingApproval);

		_personalDataEncryptionService.Encrypt(databasedModel);

		var personalData = await _personalDataRepository.CreateOrUpdateAsync(databasedModel);

		await _kycScansService.DeleteAllByPersonalDataId(personalData.Id);
		await _kycScansService.Create(request.KycScans, personalData);

		_personalDataEncryptionService.Decrypt(personalData);

		return personalData;
	}

	public async Task<IEnumerable<PersonalDataDatabaseModel>> GetUnapprovedAsync()
	{
		var list = await _personalDataRepository.GetUnapprovedAsync();

		foreach (var personalDataDatabaseModel in list)
			_personalDataEncryptionService.Decrypt(personalDataDatabaseModel);

		return list;
	}

	public async Task<PersonalDataDatabaseModel> GetByIdAsync(Guid personalDataId)
	{
		var personalData = await _personalDataRepository.FindByIdAsync(personalDataId);

		if (personalData == null)
			throw new PersonalDataNotFoundException(personalDataId);

		_personalDataEncryptionService.Decrypt(personalData);

		return personalData;
	}

	public async Task<bool> Approve(PersonalDataDatabaseModel model)
	{
		if (model.Status != PersonalDataStatus.PendingApproval)
			return false;

		await _personalDataRepository.ChangeStatusAsync(model.Id, PersonalDataStatus.Approved);

		return true;
	}

	public async Task<bool> Decline(PersonalDataDatabaseModel model)
	{
		if (model.Status != PersonalDataStatus.PendingApproval)
			return false;

		await _personalDataRepository.ChangeStatusAsync(model.Id, PersonalDataStatus.Declined);

		return true;
	}
}
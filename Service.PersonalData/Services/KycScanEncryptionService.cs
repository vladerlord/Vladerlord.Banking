using Service.PersonalData.Abstractions;
using Service.PersonalData.Exceptions;
using Shared.Grpc.PersonalData.Models;

namespace Service.PersonalData.Services;

public class KycScanEncryptionService
{
	private readonly EncryptionService _encryptionService;
	private readonly IPersonalDataRepository _personalDataRepository;

	public KycScanEncryptionService(EncryptionService encryptionService, IPersonalDataRepository personalDataRepository)
	{
		_encryptionService = encryptionService;
		_personalDataRepository = personalDataRepository;
	}

	public void EncryptCreateGrpcModel(KycScanCreateGrpcModel model, string iv)
	{
		var ivBytes = _encryptionService.IvToBytes(iv);

		model.Content = _encryptionService.Encrypt(model.Content, ivBytes);
	}

	public void DecryptGrpcModel(KycScanGrpcModel model, string iv)
	{
		var ivBytes = _encryptionService.IvToBytes(iv);

		model.Content = _encryptionService.Decrypt(model.Content, ivBytes);
	}

	public async Task DecryptGrpcModel(KycScanGrpcModel model)
	{
		var personalData = await _personalDataRepository.FindByIdAsync(model.PersonalDataId);

		if (personalData == null)
			throw new PersonalDataNotFoundException(model.PersonalDataId);

		var ivBytes = _encryptionService.IvToBytes(personalData.Iv);

		model.Content = _encryptionService.Decrypt(model.Content, ivBytes);
	}
}
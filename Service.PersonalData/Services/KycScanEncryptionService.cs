using Shared.Abstractions.Grpc.PersonalData.Models;

namespace Service.PersonalData.Services;

public class KycScanEncryptionService
{
	private readonly EncryptionService _encryptionService;

	public KycScanEncryptionService(EncryptionService encryptionService)
	{
		_encryptionService = encryptionService;
	}

	public void EncryptGrpcModel(KycScanGrpcModel model, string iv)
	{
		var ivBytes = _encryptionService.IvToBytes(iv);

		model.Content = _encryptionService.Encrypt(model.Content, ivBytes);
	}
}
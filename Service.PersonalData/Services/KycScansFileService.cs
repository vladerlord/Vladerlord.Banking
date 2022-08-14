using Service.PersonalData.Models;
using Shared.Abstractions.Grpc.PersonalData.Models;

namespace Service.PersonalData.Services;

public class KycScansFileService
{
	private readonly string _scansFolderPath;

	public KycScansFileService(string scansFolderPath)
	{
		_scansFolderPath = scansFolderPath;
	}

	public async Task SaveKycScan(KycScanGrpcModel kycScanGrpcModel, string fileName)
	{
		var path = $"{_scansFolderPath}/{fileName}{kycScanGrpcModel.FileExtension}";

		await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
		fs.Write(kycScanGrpcModel.Content, 0, kycScanGrpcModel.Content.Length);
	}

	public void DeleteKycScan(KycScanDatabaseModel kycScan)
	{
		var path = $"{_scansFolderPath}/{kycScan.FileName}{kycScan.FileExtension}";

		if (File.Exists(path))
			File.Delete(path);
	}
}
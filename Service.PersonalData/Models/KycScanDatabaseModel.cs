using Shared.Abstractions.Grpc.PersonalData.Models;

namespace Service.PersonalData.Models;

public class KycScanDatabaseModel
{
	public Guid FileName { get; set; }
	public Guid PersonalDataId { get; set; }
	public string FileExtension { get; set; }
	public string OriginalName { get; set; }

	public KycScanDatabaseModel()
	{
		FileExtension = string.Empty;
		OriginalName = string.Empty;
	}

	public KycScanDatabaseModel(Guid fileName, Guid personalDataId, string fileExtension, string originalName)
	{
		FileName = fileName;
		PersonalDataId = personalDataId;
		FileExtension = fileExtension;
		OriginalName = originalName;
	}
}

public static class KycScanGrpcRequestExtensions
{
	public static KycScanDatabaseModel ToDatabaseModel(this KycScanGrpcModel grpcModel, Guid personalDataId)
	{
		return new KycScanDatabaseModel(
			Guid.NewGuid(),
			personalDataId,
			grpcModel.FileExtension,
			grpcModel.FileName
		);
	}
}
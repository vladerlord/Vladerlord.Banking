using Shared.Grpc.PersonalData.Models;

namespace Service.PersonalData.Models;

public class KycScanDatabaseModel
{
	public Guid FileName { get; set; }
	public Guid PersonalDataId { get; set; }
	public string FileExtension { get; set; }
	public string OriginalName { get; set; }
	public string ContentType { get; set; }

	public KycScanDatabaseModel()
	{
		FileExtension = string.Empty;
		OriginalName = string.Empty;
		ContentType = string.Empty;
	}

	public KycScanDatabaseModel(Guid fileName, Guid personalDataId, string fileExtension, string originalName,
		string contentType)
	{
		FileName = fileName;
		PersonalDataId = personalDataId;
		FileExtension = fileExtension;
		OriginalName = originalName;
		ContentType = contentType;
	}

	public KycScanGrpcModel ToGrpcModel(byte[] content)
	{
		return new KycScanGrpcModel
		{
			FileName = FileName,
			PersonalDataId = PersonalDataId,
			FileExtension = FileExtension,
			OriginalName = OriginalName,
			ContentType = ContentType,
			Content = content
		};
	}
}

public static class KycScanGrpcRequestExtensions
{
	public static KycScanDatabaseModel ToDatabaseModel(this KycScanCreateGrpcModel grpcModel, Guid personalDataId)
	{
		return new KycScanDatabaseModel(
			Guid.NewGuid(),
			personalDataId,
			grpcModel.FileExtension,
			grpcModel.FileName,
			grpcModel.ContentType
		);
	}
}
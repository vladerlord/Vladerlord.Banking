using System.Runtime.Serialization;

namespace Shared.Grpc.PersonalData.Models;

[DataContract]
public class KycScanGrpcModel
{
    [DataMember(Order = 1)] public Guid FileName { get; init; }
    [DataMember(Order = 2)] public Guid PersonalDataId { get; set; }
    [DataMember(Order = 3)] public string FileExtension { get; set; }
    [DataMember(Order = 4)] public string OriginalName { get; set; }
    [DataMember(Order = 5)] public string ContentType { get; set; }
    [DataMember(Order = 6)] public byte[] Content { get; set; }

    public KycScanGrpcModel()
    {
        FileExtension = string.Empty;
        OriginalName = string.Empty;
        ContentType = string.Empty;
        Content = Array.Empty<byte>();
    }
}

[DataContract]
public class KycScanCreateGrpcModel
{
    [DataMember(Order = 1)] public string FileName { get; set; }
    [DataMember(Order = 2)] public string FileExtension { get; set; }
    [DataMember(Order = 3)] public string ContentType { get; set; }
    [DataMember(Order = 4)] public byte[] Content { get; set; }

    public KycScanCreateGrpcModel()
    {
        FileName = string.Empty;
        FileExtension = string.Empty;
        ContentType = string.Empty;
        Content = Array.Empty<byte>();
    }

    public KycScanCreateGrpcModel(string fileName, string fileExtension, string contentType, byte[] content)
    {
        FileName = fileName;
        FileExtension = fileExtension;
        ContentType = contentType;
        Content = content;
    }
}

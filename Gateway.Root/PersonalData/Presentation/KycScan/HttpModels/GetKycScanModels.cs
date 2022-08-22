using System.Runtime.Serialization;

namespace Gateway.Root.PersonalData.Presentation.KycScan.HttpModels;

[DataContract]
public class GetByIdRequest
{
    [DataMember] public Guid KycScanId { get; set; }
}

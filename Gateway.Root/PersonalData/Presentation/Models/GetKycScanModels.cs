using System.Runtime.Serialization;

namespace Gateway.Root.PersonalData.Presentation.Models;

[DataContract]
public class GetKycScanByIdHttpRequest
{
	[DataMember] public Guid KycScanId { get; set; }
}
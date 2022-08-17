using Gateway.Root.PersonalData.Domain;
using Gateway.Root.Shared;
using Shared.Grpc;

namespace Gateway.Root.PersonalData.Application;

public class ListAllUnapprovedResponse : ApplicationGrpcResponse
{
	public List<PersonalDataDto> PersonalDataList { get; }

	public ListAllUnapprovedResponse(GrpcResponseStatus status, List<PersonalDataDto> personalDataList) : base(status)
	{
		PersonalDataList = personalDataList;
	}
}
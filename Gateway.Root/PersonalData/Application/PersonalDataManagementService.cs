using Gateway.Root.PersonalData.Domain;
using Shared.Grpc;
using Shared.Grpc.PersonalData;
using Shared.Grpc.PersonalData.Contracts;

namespace Gateway.Root.PersonalData.Application;

public class PersonalDataManagementService
{
	private readonly IPersonalDataGrpcService _personalDataGrpcService;
	private readonly KycScanLinkBuilder _kycScanLinkBuilder;

	public PersonalDataManagementService(IPersonalDataGrpcService personalDataGrpcService,
		KycScanLinkBuilder kycScanLinkBuilder)
	{
		_personalDataGrpcService = personalDataGrpcService;
		_kycScanLinkBuilder = kycScanLinkBuilder;
	}

	public async Task<ListAllUnapprovedResponse> ListAllUnapproved()
	{
		var result = await _personalDataGrpcService.ListAllUnapprovedPersonalDataAsync();
		var dtoList = result.PersonalDataList
			.Select(personalDataGrpcModel => personalDataGrpcModel.ToDto())
			.ToList();

		return new ListAllUnapprovedResponse(result.Status, dtoList);
	}

	public async Task<GetPersonalDataByIdResponse> GetPersonalDataById(Guid personalDataId)
	{
		var grpcResponse =
			await _personalDataGrpcService.GetByIdAsync(new GetPersonalDataByIdGrpcRequest(personalDataId));

		var kycScansLinks = (from kycScanId in grpcResponse.KycScansIds
			select _kycScanLinkBuilder.BuildLinkToKycScan(kycScanId)).ToList();

		return new GetPersonalDataByIdResponse(
			grpcResponse.GrpcResponse.Status,
			grpcResponse.PersonalData?.ToDto(),
			kycScansLinks
		);
	}

	public async Task<GrpcResponseStatus> ApproveAsync(Guid personalData)
	{
		var result = await _personalDataGrpcService.ApprovePersonalDataAsync(new ApprovePersonalDataGrpcRequest
		{
			PersonalDataId = personalData
		});

		return result.GrpcResponse.Status;
	}

	public async Task<GrpcResponseStatus> DeclineAsync(Guid personalData)
	{
		var result = await _personalDataGrpcService.DeclinePersonalDataAsync(new DeclinePersonalDataGrpcRequest
		{
			PersonalDataId = personalData
		});

		return result.GrpcResponse.Status;
	}
}

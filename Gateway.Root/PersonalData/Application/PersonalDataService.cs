using Gateway.Root.PersonalData.Domain;
using Shared.Abstractions.Grpc.PersonalData;
using Shared.Abstractions.Grpc.PersonalData.Contracts;

namespace Gateway.Root.PersonalData.Application;

public class PersonalDataService
{
	private readonly IPersonalDataGrpcService _personalDataGrpcService;

	public PersonalDataService(IPersonalDataGrpcService personalDataGrpcService)
	{
		_personalDataGrpcService = personalDataGrpcService;
	}

	public async Task<ApplyPersonalDataGrpcResponse> SendPersonalDataConfirmationRequest(
		PersonalDataConfirmationDto dto)
	{
		var kycScansGrpcModels = await dto.ToKycScanGrpcModels();
		var request = new ApplyPersonalDataGrpcRequest(dto.ToPersonalDataGrpcModel(), kycScansGrpcModels);

		return await _personalDataGrpcService.ApplyPersonalDataAsync(request);
	}
}

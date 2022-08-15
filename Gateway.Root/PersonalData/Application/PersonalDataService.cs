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

	public async Task<PersonalDataConfirmationResponseDto> SendPersonalDataConfirmationRequest(
		PersonalDataConfirmationDto dto)
	{
		var kycScansGrpcModels = await dto.ToKycScanGrpcModels();
		var request = new ApplyPersonalDataGrpcRequest(dto.ToPersonalDataGrpcModel(), kycScansGrpcModels);

		var response = await _personalDataGrpcService.ApplyPersonalDataAsync(request);

		return new PersonalDataConfirmationResponseDto(response.Status, response.PersonalData?.ToDto());
	}
}
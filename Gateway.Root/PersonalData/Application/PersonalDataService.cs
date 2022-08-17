using Gateway.Root.PersonalData.Domain;
using Shared.Grpc.PersonalData;
using Shared.Grpc.PersonalData.Contracts;

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
		var kycScansCreateGrpcModels = await dto.ToKycScanCreateGrpcModels();
		var request = new ApplyPersonalDataGrpcRequest(dto.ToPersonalDataCreateGrpcModel(), kycScansCreateGrpcModels);

		var response = await _personalDataGrpcService.ApplyPersonalDataAsync(request);

		return new PersonalDataConfirmationResponseDto(response.Status, response.PersonalData?.ToDto());
	}
}
using Gateway.Root.PersonalData.Domain;
using Gateway.Root.Shared;
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

    public async Task<GrpcAppResponse<PersonalDataDto?>> SendPersonalDataConfirmationRequest(
        PersonalDataConfirmationDto dto)
    {
        var kycScansCreateGrpcModels = await dto.ToKycScanCreateGrpcModels();
        var request = new ApplyPersonalDataGrpcRequest
        {
            PersonalData = dto.ToPersonalDataCreateGrpcModel(),
            KycScans = kycScansCreateGrpcModels
        };

        var response = await _personalDataGrpcService.ApplyPersonalDataAsync(request);

        return new GrpcAppResponse<PersonalDataDto?>
        {
            GrpcStatus = response.GrpcResponse,
            Content = response.PersonalData?.ToDto()
        };
    }
}

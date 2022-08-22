using Gateway.Root.PersonalData.Domain;
using Gateway.Root.Shared;
using Shared.Grpc.PersonalData;
using Shared.Grpc.PersonalData.Contracts;

namespace Gateway.Root.PersonalData.Application;

public class PersonalDataManagementService
{
    private readonly IPersonalDataGrpcService _personalDataGrpcService;

    public PersonalDataManagementService(IPersonalDataGrpcService personalDataGrpcService)
    {
        _personalDataGrpcService = personalDataGrpcService;
    }

    public async Task<GrpcAppResponse<IEnumerable<PersonalDataDto>>> ListAllUnapproved()
    {
        var grpcResponse = await _personalDataGrpcService.ListAllUnapprovedPersonalDataAsync();
        var dtoList = grpcResponse.PersonalDataList
            .Select(personalDataGrpcModel => personalDataGrpcModel.ToDto())
            .ToList();

        return new GrpcAppResponse<IEnumerable<PersonalDataDto>>
        {
            GrpcStatus = grpcResponse.GrpcResponse,
            Content = dtoList
        };
    }

    public async Task<GrpcAppResponse<PersonalDataDto?>> GetPersonalDataById(Guid personalDataId)
    {
        var grpcRequest = new GetPersonalDataByIdGrpcRequest { PersonalDataId = personalDataId };

        var grpcResponse = await _personalDataGrpcService.GetByIdAsync(grpcRequest);

        return new GrpcAppResponse<PersonalDataDto?>
        {
            GrpcStatus = grpcResponse.GrpcResponse,
            Content = grpcResponse.PersonalData?.ToDto(grpcResponse.KycScansIds)
        };
    }

    public async Task<GrpcAppResponse> ApproveAsync(Guid personalData)
    {
        var result = await _personalDataGrpcService.ApprovePersonalDataAsync(new ApprovePersonalDataGrpcRequest
        {
            PersonalDataId = personalData
        });

        return new GrpcAppResponse { GrpcStatus = result.GrpcResponse };
    }

    public async Task<GrpcAppResponse> DeclineAsync(Guid personalData)
    {
        var result = await _personalDataGrpcService.DeclinePersonalDataAsync(new DeclinePersonalDataGrpcRequest
        {
            PersonalDataId = personalData
        });

        return new GrpcAppResponse { GrpcStatus = result.GrpcResponse };
    }
}

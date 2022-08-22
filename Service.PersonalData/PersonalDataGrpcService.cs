using Service.PersonalData.Exceptions;
using Service.PersonalData.Models;
using Service.PersonalData.Services;
using Shared.Grpc;
using Shared.Grpc.PersonalData;
using Shared.Grpc.PersonalData.Contracts;

namespace Service.PersonalData;

public class PersonalDataGrpcService : IPersonalDataGrpcService
{
    private readonly PersonalDataService _personalDataService;
    private readonly KycScansService _kycScansService;

    public PersonalDataGrpcService(PersonalDataService personalDataService, KycScansService kycScansService)
    {
        _personalDataService = personalDataService;
        _kycScansService = kycScansService;
    }

    public async Task<ApplyPersonalDataGrpcResponse> ApplyPersonalDataAsync(ApplyPersonalDataGrpcRequest request)
    {
        var existentPersonalData = await _personalDataService.FindByUserId(request.PersonalData.UserId);

        if (existentPersonalData == null)
            return new ApplyPersonalDataGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.NotFound }
            };

        switch (existentPersonalData.Status)
        {
            case PersonalDataStatus.PendingApproval:
                return new ApplyPersonalDataGrpcResponse
                {
                    GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.AlreadyInProcess },
                    PersonalData = existentPersonalData.ToGrpcModel()
                };
            case PersonalDataStatus.Approved:
                return new ApplyPersonalDataGrpcResponse
                {
                    GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.AlreadyApproved }
                };
            case PersonalDataStatus.Declined:
            default:
            {
                var personalData = await _personalDataService.ApplyPersonalDataAsync(request);

                return new ApplyPersonalDataGrpcResponse
                {
                    GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok },
                    PersonalData = personalData.ToGrpcModel()
                };
            }
        }
    }

    public async Task<ListAllUnapprovedPersonalDataGrpcResponse> ListAllUnapprovedPersonalDataAsync()
    {
        var list = await _personalDataService.GetUnapprovedAsync();
        var result = list.Select(personalDataDatabaseModel => personalDataDatabaseModel.ToGrpcModel()).ToList();

        return new ListAllUnapprovedPersonalDataGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok },
            PersonalDataList = result
        };
    }

    public async Task<GetPersonalDataByIdGrpcResponse> GetByIdAsync(GetPersonalDataByIdGrpcRequest request)
    {
        PersonalDataDatabaseModel personalData;

        try
        {
            personalData = await _personalDataService.GetByIdAsync(request.PersonalDataId);
        }
        catch (PersonalDataNotFoundException)
        {
            return new GetPersonalDataByIdGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.NotFound }
            };
        }

        var kycScans = await _kycScansService.GetAllByPersonalData(personalData);
        var kycScansIds = kycScans.Select(kycScanGrpcModel => kycScanGrpcModel.FileName).ToList();

        var response = new GetPersonalDataByIdGrpcResponse
        {
            GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok },
            PersonalData = personalData.ToGrpcModel(),
            KycScansIds = kycScansIds
        };

        return response;
    }

    public async Task<ApprovePersonalDataGrpcResponse> ApprovePersonalDataAsync(ApprovePersonalDataGrpcRequest request)
    {
        PersonalDataDatabaseModel personalData;

        try
        {
            personalData = await _personalDataService.GetByIdAsync(request.PersonalDataId);
        }
        catch (PersonalDataNotFoundException)
        {
            return new ApprovePersonalDataGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.NotFound }
            };
        }

        var result = await _personalDataService.Approve(personalData);

        return new ApprovePersonalDataGrpcResponse
        {
            GrpcResponse = new GrpcResponse
            {
                Status = result ? GrpcResponseStatus.Ok : GrpcResponseStatus.AlreadyChanged
            }
        };
    }

    public async Task<DeclinePersonalDataGrpcResponse> DeclinePersonalDataAsync(DeclinePersonalDataGrpcRequest request)
    {
        PersonalDataDatabaseModel personalData;

        try
        {
            personalData = await _personalDataService.GetByIdAsync(request.PersonalDataId);
        }
        catch (PersonalDataNotFoundException)
        {
            return new DeclinePersonalDataGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.NotFound }
            };
        }

        var result = await _personalDataService.Decline(personalData);

        return new DeclinePersonalDataGrpcResponse
        {
            GrpcResponse = new GrpcResponse
            {
                Status = result ? GrpcResponseStatus.Ok : GrpcResponseStatus.AlreadyChanged
            }
        };
    }
}

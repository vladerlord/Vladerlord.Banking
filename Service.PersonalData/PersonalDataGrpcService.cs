using Service.PersonalData.Abstractions;
using Service.PersonalData.Exceptions;
using Service.PersonalData.Models;
using Shared.Grpc;
using Shared.Grpc.PersonalData;
using Shared.Grpc.PersonalData.Contracts;
using Shared.Grpc.PersonalData.Models;

namespace Service.PersonalData;

public class PersonalDataGrpcService : IPersonalDataGrpcService
{
    private readonly IPersonalDataService _personalDataService;
    private readonly IKycScansService _kycScansService;
    private readonly ILogger<PersonalDataGrpcService> _logger;

    public PersonalDataGrpcService(IPersonalDataService personalDataService, IKycScansService kycScansService,
        ILogger<PersonalDataGrpcService> logger)
    {
        _personalDataService = personalDataService;
        _kycScansService = kycScansService;
        _logger = logger;
    }

    public async Task<ApplyPersonalDataGrpcResponse> ApplyPersonalDataAsync(ApplyPersonalDataGrpcRequest request)
    {
        try
        {
            var existentPersonalData = await _personalDataService.FindByUserId(request.PersonalData.UserId);

            if (existentPersonalData != null)
            {
                if (existentPersonalData.Status == PersonalDataStatus.PendingApproval)
                    return new ApplyPersonalDataGrpcResponse
                    {
                        GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.AlreadyInProcess },
                        PersonalData = existentPersonalData.ToGrpcModel()
                    };

                if (existentPersonalData.Status == PersonalDataStatus.Approved)
                    return new ApplyPersonalDataGrpcResponse
                    {
                        GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.AlreadyApproved }
                    };
            }

            var personalData = await _personalDataService.ApplyPersonalDataAsync(request);

            return new ApplyPersonalDataGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok },
                PersonalData = personalData.ToGrpcModel()
            };
        }
        catch (Exception e)
        {
            _logger.LogError("Error when applying user id: {UserId}. Error: {Error}", request.PersonalData.UserId,
                e.ToString());

            return new ApplyPersonalDataGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Error }
            };
        }
    }

    public async Task<ListAllUnapprovedPersonalDataGrpcResponse> ListAllUnapprovedPersonalDataAsync()
    {
        try
        {
            var list = await _personalDataService.GetUnapprovedAsync();
            var result = list.Select(personalDataDatabaseModel => personalDataDatabaseModel.ToGrpcModel()).ToList();

            return new ListAllUnapprovedPersonalDataGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Ok },
                PersonalDataList = result
            };
        }
        catch (Exception e)
        {
            _logger.LogError("{Error}", e.ToString());

            return new ListAllUnapprovedPersonalDataGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Error },
                PersonalDataList = new List<PersonalDataGrpcModel>()
            };
        }
    }

    public async Task<GetPersonalDataByIdGrpcResponse> GetByIdAsync(GetPersonalDataByIdGrpcRequest request)
    {
        try
        {
            var personalData = await _personalDataService.GetByIdAsync(request.PersonalDataId);
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
        catch (PersonalDataNotFoundException)
        {
            return new GetPersonalDataByIdGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.NotFound }
            };
        }
        catch (Exception e)
        {
            _logger.LogError("Error when trying to get by id: {Id}. Error: {Error}", request.PersonalDataId,
                e.ToString());

            return new GetPersonalDataByIdGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Error }
            };
        }
    }

    public async Task<GetByUserIdPersonalDataByIdGrpcResponse> GetByUserIdAsync(
        GetByUserIdPersonalDataByIdGrpcRequest request)
    {
        try
        {
            var personalData = await _personalDataService.FindByUserId(request.UserId);

            return new GetByUserIdPersonalDataByIdGrpcResponse
            {
                GrpcResponse = new GrpcResponse
                {
                    Status = personalData != null ? GrpcResponseStatus.Ok : GrpcResponseStatus.NotFound
                },
                PersonalData = personalData?.ToGrpcModel()
            };
        }
        catch (Exception e)
        {
            _logger.LogError("User id: {UserId}. Error: {Error}", request.UserId, e.ToString());

            return new GetByUserIdPersonalDataByIdGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Error },
            };
        }
    }

    public async Task<ApprovePersonalDataGrpcResponse> ApprovePersonalDataAsync(ApprovePersonalDataGrpcRequest request)
    {
        try
        {
            var personalData = await _personalDataService.GetByIdAsync(request.PersonalDataId);
            var result = await _personalDataService.Approve(personalData);

            return new ApprovePersonalDataGrpcResponse
            {
                GrpcResponse = new GrpcResponse
                {
                    Status = result ? GrpcResponseStatus.Ok : GrpcResponseStatus.AlreadyChanged
                }
            };
        }
        catch (PersonalDataNotFoundException)
        {
            return new ApprovePersonalDataGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.NotFound }
            };
        }
        catch (Exception e)
        {
            _logger.LogError("Approve: {Id}. Error: {Error}", request.PersonalDataId, e.ToString());

            return new ApprovePersonalDataGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Error }
            };
        }
    }

    public async Task<DeclinePersonalDataGrpcResponse> DeclinePersonalDataAsync(DeclinePersonalDataGrpcRequest request)
    {
        try
        {
            var personalData = await _personalDataService.GetByIdAsync(request.PersonalDataId);
            var result = await _personalDataService.Decline(personalData);

            return new DeclinePersonalDataGrpcResponse
            {
                GrpcResponse = new GrpcResponse
                {
                    Status = result ? GrpcResponseStatus.Ok : GrpcResponseStatus.AlreadyChanged
                }
            };
        }
        catch (PersonalDataNotFoundException)
        {
            return new DeclinePersonalDataGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.NotFound }
            };
        }
        catch (Exception e)
        {
            _logger.LogError("Decline: {Id}. Error: {Error}", request.PersonalDataId, e.ToString());

            return new DeclinePersonalDataGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Error }
            };
        }
    }
}

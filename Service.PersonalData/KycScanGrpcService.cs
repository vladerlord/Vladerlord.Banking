using Service.PersonalData.Abstractions;
using Shared.Grpc;
using Shared.Grpc.PersonalData;
using Shared.Grpc.PersonalData.Contracts;

namespace Service.PersonalData;

public class KycScanGrpcService : IKycScanGrpcService
{
    private readonly IKycScansService _kycScansService;
    private readonly ILogger<KycScanGrpcService> _logger;

    public KycScanGrpcService(IKycScansService kycScansService, ILogger<KycScanGrpcService> logger)
    {
        _kycScansService = kycScansService;
        _logger = logger;
    }

    public async Task<FindKycScanByIdGrpcResponse> FindByIdAsync(FindKycScanByIdGrpcRequest request)
    {
        try
        {
            var kycScan = await _kycScansService.FindById(request.Id);

            return new FindKycScanByIdGrpcResponse
            {
                GrpcResponse = new GrpcResponse
                {
                    Status = kycScan == null ? GrpcResponseStatus.NotFound : GrpcResponseStatus.Ok
                },
                KycScan = kycScan
            };
        }
        catch (Exception e)
        {
            _logger.LogError("Error when trying to find kyc scan by id: {Id}. Error: {Error}", request.Id,
                e.ToString());

            return new FindKycScanByIdGrpcResponse
            {
                GrpcResponse = new GrpcResponse { Status = GrpcResponseStatus.Error }
            };
        }
    }
}

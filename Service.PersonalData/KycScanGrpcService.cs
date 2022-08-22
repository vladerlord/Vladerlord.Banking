using Service.PersonalData.Services;
using Shared.Grpc;
using Shared.Grpc.PersonalData;
using Shared.Grpc.PersonalData.Contracts;

namespace Service.PersonalData;

public class KycScanGrpcService : IKycScanGrpcService
{
    private readonly KycScansService _kycScansService;

    public KycScanGrpcService(KycScansService kycScansService)
    {
        _kycScansService = kycScansService;
    }

    public async Task<FindKycScanByIdGrpcResponse> FindByIdAsync(FindKycScanByIdGrpcRequest request)
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
}

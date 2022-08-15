using Service.PersonalData.Services;
using Shared.Abstractions.Grpc;
using Shared.Abstractions.Grpc.PersonalData;
using Shared.Abstractions.Grpc.PersonalData.Contracts;
using Shared.Abstractions.Grpc.PersonalData.Models;

namespace Service.PersonalData;

public class PersonalDataGrpcService : IPersonalDataGrpcService
{
	private readonly PersonalDataService _personalDataService;

	public PersonalDataGrpcService(PersonalDataService personalDataService)
	{
		_personalDataService = personalDataService;
	}

	public async Task<ApplyPersonalDataGrpcResponse> ApplyPersonalDataAsync(ApplyPersonalDataGrpcRequest request)
	{
		var status = GrpcResponseStatus.Ok;
		PersonalDataGrpcModel? model = null;

		// todo, if exists and have status WaitingOnApproval = return AlreadyExist

		try
		{
			var personalData = await _personalDataService.ApplyPersonalDataAsync(request);
			model = personalData.ToGrpcModel();
		}
		catch
		{
			status = GrpcResponseStatus.Error;
		}

		return new ApplyPersonalDataGrpcResponse(status, model);
	}
}

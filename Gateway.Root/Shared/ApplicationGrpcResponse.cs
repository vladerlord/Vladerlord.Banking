using Shared.Grpc;

namespace Gateway.Root.Shared;

public class ApplicationGrpcResponse
{
	public string Status { get; }
	public int StatusCode { get; }

	protected ApplicationGrpcResponse(GrpcResponseStatus status)
	{
		Status = status.ToString();
		StatusCode = status.ToHttpCode();
	}
}
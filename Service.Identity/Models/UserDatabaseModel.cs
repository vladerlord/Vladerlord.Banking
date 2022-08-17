using Shared.Abstractions;
using Shared.Abstractions.MessageBus.Identity;
using Shared.Grpc.Identity.Contracts;
using Shared.Grpc.Identity.Models;

namespace Service.Identity.Models;

public class UserDatabaseModel
{
	public Guid Id { get; }
	public string Email { get; }
	public string Password { get; }
	public UserStatus Status { get; }

	public UserDatabaseModel()
	{
		Email = string.Empty;
		Password = string.Empty;
	}

	public UserDatabaseModel(Guid id, string email, string password, UserStatus status)
	{
		Id = id;
		Email = email;
		Password = password;
		Status = status;
	}

	public UserGrpcModel ToGrpcModel()
	{
		return new UserGrpcModel
		{
			Id = Id,
			Email = Email
		};
	}

	public UserCreatedEvent ToCreatedEvent(string confirmationUrl)
	{
		return new UserCreatedEvent
		{
			Id = Id,
			Email = Email,
			ConfirmationLink = confirmationUrl
		};
	}
}

public static class RegisterGrpcRequestExtensions
{
	public static UserDatabaseModel ToDatabaseModel(this RegisterUserGrpcRequest request, string hashedPassword,
		UserStatus status)
	{
		return new UserDatabaseModel(Guid.NewGuid(), request.Email, hashedPassword, status);
	}
}
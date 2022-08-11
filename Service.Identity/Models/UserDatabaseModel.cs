using Shared.Abstractions.Grpc.Identity.Contracts;
using Shared.Abstractions.Grpc.Identity.Models;
using Shared.Abstractions.MessageBus.Identity;

namespace Service.Identity.Models;

public class UserDatabaseModel
{
	public Guid Id { get; }
	public string Email { get; }
	public string Password { get; }
	public string Iv { get; }
	public UserStatus Status { get; }

	public UserDatabaseModel()
	{
		Email = string.Empty;
		Password = string.Empty;
		Iv = string.Empty;
	}

	public UserDatabaseModel(Guid id, string email, string password, string iv, UserStatus status)
	{
		Id = id;
		Email = email;
		Password = password;
		Iv = iv;
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

public enum UserStatus
{
	Created,
	Confirmed,
	Approved,
	Admin
}

public static class RegisterGrpcRequestExtensions
{
	public static UserDatabaseModel ToDatabaseModel(this RegisterUserGrpcRequest request, string hashedPassword,
		string iv, UserStatus status)
	{
		return new UserDatabaseModel(Guid.NewGuid(), request.Email, hashedPassword, iv, status);
	}
}

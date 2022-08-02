using Shared.Grpc.Identity.Contracts;
using Shared.Grpc.Identity.Models;

namespace IdentityGrpc.Models;

public class UserDatabaseModel
{
    public Guid Id { get; }
    public string Email { get; }
    public string Password { get; }
    public string Iv { get; }

    public UserDatabaseModel(Guid id, string email, string password, string iv)
    {
        Id = id;
        Email = email;
        Password = password;
        Iv = iv;
    }

    public UserGrpcModel ToGrpcModel()
    {
        return new UserGrpcModel(Id, Email);
    }
}

public static class UserDatabaseSchema
{
    public static string Table => "users";

    public static class Columns
    {
        public static string Id { get; } = nameof(UserDatabaseModel.Id).ToSnakeCase();
        public static string Email { get; } = nameof(UserDatabaseModel.Email).ToSnakeCase();
        public static string Password { get; } = nameof(UserDatabaseModel.Password).ToSnakeCase();
        public static string Iv { get; } = nameof(UserDatabaseModel.Iv).ToSnakeCase();
    }
}

public static class RegisterGrpcRequestExtensions
{
    public static UserDatabaseModel ToDatabaseModel(this RegisterUserGrpcRequest request, string hashedPassword,
        string iv)
    {
        return new UserDatabaseModel(Guid.NewGuid(), request.Email, hashedPassword, iv);
    }
}
using System.Runtime.Serialization;
using Shared.Abstractions;

namespace Shared.Grpc.BankAccount.Model;

[DataContract]
public class MoneyValueGrpcModel
{
    [DataMember(Order = 1)] public long Value { get; init; }
}

public static class MoneyValueGrpcModelExtensions
{
    public static MoneyValueGrpcModel ToGrpc(this MoneyValue moneyValue)
    {
        return new MoneyValueGrpcModel
        {
            Value = moneyValue.Value
        };
    }

    public static MoneyValue ToDto(this MoneyValueGrpcModel grpcModel)
    {
        return new MoneyValue(grpcModel.Value);
    }
}

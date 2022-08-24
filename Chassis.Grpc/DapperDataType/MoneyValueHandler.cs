using System.Data;
using Dapper;
using Shared.Abstractions;

namespace Chassis.Grpc.DapperDataType;

public class MoneyValueHandler : SqlMapper.TypeHandler<MoneyValue>
{
    public override void SetValue(IDbDataParameter parameter, MoneyValue value)
    {
        parameter.Value = value.Value;
    }

    public override MoneyValue Parse(object value)
    {
        return new MoneyValue((long)value);
    }
}

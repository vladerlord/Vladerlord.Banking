namespace Shared.Abstractions;

public struct MoneyValue
{
    private const double Multiplier = 1_000_000;

    public long Value { get; }

    public MoneyValue(long value)
    {
        Value = value;
    }
    
    public MoneyValue(double value)
    {
        Value = Convert.ToInt64(value * Multiplier);
    }

    public decimal GetAsDecimal()
    {
        return Convert.ToDecimal(Value / Multiplier);
    }
}

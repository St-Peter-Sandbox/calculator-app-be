namespace CalculatorAppBackend.Domain;

public record CalculationRequest(decimal Operand1, decimal Operand2, string Operation);

public record CalculationResult(decimal? Result, string Equation, bool IsSuccess, string? ErrorMessage);

public record HistoryItem(string Id, string Equation, decimal Result, DateTime Timestamp);

public interface ICalculatorService
{
    CalculationResult Calculate(decimal operand1, decimal operand2, string operation);
}

public interface ICalculationHistory
{
    IReadOnlyList<HistoryItem> GetAll();
    void Add(HistoryItem item);
    void Clear();
}

public class CalculatorService : ICalculatorService
{
    public CalculationResult Calculate(decimal operand1, decimal operand2, string operation)
    {
        var op = NormalizeOperation(operation);

        switch (op)
        {
            case "add":
                {
                    var res = operand1 + operand2;
                    return new CalculationResult(res, $"{FormatNumber(operand1)} + {FormatNumber(operand2)} = {FormatNumber(res)}", true, null);
                }
            case "subtract":
                {
                    var res = operand1 - operand2;
                    return new CalculationResult(res, $"{FormatNumber(operand1)} - {FormatNumber(operand2)} = {FormatNumber(res)}", true, null);
                }
            case "multiply":
                {
                    var res = operand1 * operand2;
                    return new CalculationResult(res, $"{FormatNumber(operand1)} × {FormatNumber(operand2)} = {FormatNumber(res)}", true, null);
                }
            case "divide":
                {
                    if (operand2 == 0)
                    {
                        return new CalculationResult(null, $"{FormatNumber(operand1)} ÷ 0", false, "Cannot divide by zero");
                    }
                    var res = operand1 / operand2;
                    return new CalculationResult(res, $"{FormatNumber(operand1)} ÷ {FormatNumber(operand2)} = {FormatNumber(res)}", true, null);
                }
            default:
                return new CalculationResult(null, string.Empty, false, $"Unsupported operation: {operation}");
        }
    }

    private static string NormalizeOperation(string op)
    {
        return op.Trim().ToLowerInvariant() switch
        {
            "+" or "add" or "plus" => "add",
            "-" or "subtract" or "minus" => "subtract",
            "*" or "x" or "×" or "multiply" or "times" => "multiply",
            "/" or "÷" or "divide" => "divide",
            _ => op.Trim().ToLowerInvariant()
        };
    }

    private static string FormatNumber(decimal value)
    {
        return (value % 1 == 0) ? ((long)value).ToString() : value.ToString("G29");
    }
}

public class CalculationHistory : ICalculationHistory
{
    private readonly List<HistoryItem> _items = new();
    private readonly object _lock = new();

    public IReadOnlyList<HistoryItem> GetAll()
    {
        lock (_lock)
        {
            return _items.OrderByDescending(x => x.Timestamp).ToList();
        }
    }

    public void Add(HistoryItem item)
    {
        lock (_lock)
        {
            _items.Add(item);
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _items.Clear();
        }
    }
}

using CalculatorAppBackend.Domain;
using Xunit;

namespace CalculatorAppBackend.Tests;

public class CalculatorServiceTests
{
    private readonly CalculatorService _service = new();

    [Fact]
    public void Addition_ComputesCorrectResult()
    {
        var result = _service.Calculate(5, 3, "add");

        Assert.True(result.IsSuccess);
        Assert.Equal(8m, result.Result);
        Assert.Equal("5 + 3 = 8", result.Equation);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Subtraction_ComputesCorrectResult()
    {
        var result = _service.Calculate(10, 4, "subtract");

        Assert.True(result.IsSuccess);
        Assert.Equal(6m, result.Result);
        Assert.Equal("10 - 4 = 6", result.Equation);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Multiplication_ComputesCorrectResult()
    {
        var result = _service.Calculate(6, 7, "multiply");

        Assert.True(result.IsSuccess);
        Assert.Equal(42m, result.Result);
        Assert.Equal("6 × 7 = 42", result.Equation);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Division_ComputesCorrectResult()
    {
        var result = _service.Calculate(20, 4, "divide");

        Assert.True(result.IsSuccess);
        Assert.Equal(5m, result.Result);
        Assert.Equal("20 ÷ 4 = 5", result.Equation);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void DivisionByZero_ReturnsClearError()
    {
        var result = _service.Calculate(20, 0, "divide");

        Assert.False(result.IsSuccess);
        Assert.Null(result.Result);
        Assert.Equal("Cannot divide by zero", result.ErrorMessage);
        Assert.Equal("20 ÷ 0", result.Equation);
    }

    [Theory]
    [InlineData("+")]
    [InlineData("plus")]
    public void SymbolAndAliasOperators_AreSupported(string op)
    {
        var result = _service.Calculate(2, 3, op);
        Assert.True(result.IsSuccess);
        Assert.Equal(5m, result.Result);
    }

    [Fact]
    public void DecimalPrecision_IsPreserved()
    {
        var result = _service.Calculate(10, 4, "/");
        Assert.True(result.IsSuccess);
        Assert.Equal(2.5m, result.Result);
        Assert.Equal("10 ÷ 4 = 2.5", result.Equation);
    }

    [Fact]
    public void UnsupportedOperation_ReturnsFailure()
    {
        var result = _service.Calculate(1, 1, "modulo");
        Assert.False(result.IsSuccess);
        Assert.Contains("Unsupported operation", result.ErrorMessage);
    }

    [Fact]
    public void History_Add_GetAll_And_Clear()
    {
        var history = new CalculationHistory();
        Assert.Empty(history.GetAll());

        history.Add(new HistoryItem("1", "5 + 3 = 8", 8m, DateTime.UtcNow.AddMinutes(-1)));
        history.Add(new HistoryItem("2", "10 - 4 = 6", 6m, DateTime.UtcNow));

        var items = history.GetAll();
        Assert.Equal(2, items.Count);
        Assert.Equal("2", items[0].Id); // Most recent first

        history.Clear();
        Assert.Empty(history.GetAll());
    }
}

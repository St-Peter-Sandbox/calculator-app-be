using System.Net;
using System.Net.Http.Json;
using CalculatorAppBackend.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CalculatorAppBackend.Tests;

public class CalculatorEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CalculatorEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Calculate_HappyPath_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var request = new CalculationRequest(5, 3, "add");

        var response = await client.PostAsJsonAsync("/api/calculate", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<CalculationResult>();
        Assert.NotNull(body);
        Assert.True(body!.IsSuccess);
        Assert.Equal(8m, body.Result);
        Assert.Equal("5 + 3 = 8", body.Equation);
    }

    [Fact]
    public async Task Calculate_DivisionByZero_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();
        var request = new CalculationRequest(20, 0, "divide");

        var response = await client.PostAsJsonAsync("/api/calculate", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<CalculationResult>();
        Assert.NotNull(body);
        Assert.False(body!.IsSuccess);
        Assert.Equal("Cannot divide by zero", body.ErrorMessage);
    }

    [Fact]
    public async Task History_Lifecycle_ReturnsItemsAndClears()
    {
        var client = _factory.CreateClient();

        // 1. Clear history first
        var deleteResponse = await client.DeleteAsync("/api/history");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // 2. Perform a calculation
        await client.PostAsJsonAsync("/api/calculate", new CalculationRequest(6, 7, "multiply"));

        // 3. Query history
        var historyResponse = await client.GetAsync("/api/history");
        Assert.Equal(HttpStatusCode.OK, historyResponse.StatusCode);

        var items = await historyResponse.Content.ReadFromJsonAsync<List<HistoryItem>>();
        Assert.NotNull(items);
        Assert.NotEmpty(items!);
        Assert.Contains(items!, item => item.Result == 42m && item.Equation.Contains("6 × 7"));
    }
}

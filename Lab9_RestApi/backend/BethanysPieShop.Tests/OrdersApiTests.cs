using System.Net;
using System.Text.Json;
using BethanysPieShop.Models;
using Guts.Client.XUnit;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;

namespace BethanysPieShop.Tests;

[ExerciseTestClass("dotnet2", "9-RestApi", "BethanysPieShop",
    @"BethanysPieShop\Controllers\Api\OrdersApiController.cs;BethanysPieShop\Models\Order.cs;BethanysPieShop\Program.cs")]
public class OrdersApiTests
{
    [MonitoredFact]
    public async Task _01_GetOrders_ReturnsOkWithAllOrders()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/orders");

        Assert.True(response.StatusCode == HttpStatusCode.OK,
            $"Expected GET /api/orders to return 200 OK, but received {response.StatusCode}. Ensure OrdersApiController has [Route(\"api/orders\")] and an action for GET.");

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        Assert.True(doc.RootElement.GetArrayLength() > 0, "The returned list of orders should contain at least one order.");

        var firstOrder = doc.RootElement[0];
        Assert.True(firstOrder.TryGetProperty("orderId", out _) || firstOrder.TryGetProperty("OrderId", out _),
            "Returned order JSON should include 'orderId'.");
        Assert.True(firstOrder.TryGetProperty("firstName", out _) || firstOrder.TryGetProperty("FirstName", out _),
            "Returned order JSON should include 'firstName'.");
        Assert.True(firstOrder.TryGetProperty("orderTotal", out _) || firstOrder.TryGetProperty("OrderTotal", out _),
            "Returned order JSON should include 'orderTotal'.");
    }

    [MonitoredFact]
    public async Task _02_ProcessOrder_ExistingOrder_UpdatesStatusAndReturnsNoContent()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        var patchResponse = await client.PatchAsync("/api/orders/1/process", null);

        Assert.True(patchResponse.StatusCode == HttpStatusCode.NoContent,
            $"Expected PATCH /api/orders/1/process to return 204 NoContent, but received {patchResponse.StatusCode}.");

        // Verify status was updated via GET
        var getResponse = await client.GetAsync("/api/orders");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var json = await getResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var order1 = doc.RootElement.EnumerateArray()
            .FirstOrDefault(e => (e.TryGetProperty("orderId", out var idProp) && idProp.GetInt32() == 1) ||
                                 (e.TryGetProperty("OrderId", out var idPropCap) && idPropCap.GetInt32() == 1));

        Assert.False(order1.ValueKind == JsonValueKind.Undefined, "Order with ID 1 should exist in orders response.");

        var foundProcessed = order1.TryGetProperty("orderProcessed", out var processedProp)
            ? processedProp.GetBoolean()
            : order1.GetProperty("OrderProcessed").GetBoolean();

        Assert.True(foundProcessed, "OrderProcessed should be true after marking the order as processed.");
    }

    [MonitoredFact]
    public async Task _03_ProcessOrder_NonExistingOrder_ReturnsNotFound()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        // Ensure the endpoint exists and returns 204 NoContent for an existing order first
        var existingOrderResponse = await client.PatchAsync("/api/orders/1/process", null);
        Assert.True(existingOrderResponse.StatusCode == HttpStatusCode.NoContent,
            "PATCH /api/orders/{id}/process must successfully return 204 NoContent for an existing order before testing non-existing orders.");

        var response = await client.PatchAsync("/api/orders/99999/process", null);

        Assert.True(response.StatusCode == HttpStatusCode.NotFound,
            $"Expected PATCH /api/orders/99999/process to return 404 NotFound, but received {response.StatusCode}.");
    }

    [MonitoredFact]
    public async Task _04_Cors_PolicyConfigured()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = factory.CreateClient();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/orders");
        request.Headers.Add("Origin", "http://localhost:5500");

        var response = await client.SendAsync(request);

        Assert.True(response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values),
            "CORS policy is not active or did not return 'Access-Control-Allow-Origin' header for http://localhost:5500. Did you register and use the CORS middleware in Program.cs?");

        var allowedOrigin = values.FirstOrDefault();
        Assert.True(allowedOrigin == "*" || allowedOrigin == "http://localhost:5500",
            $"Expected Access-Control-Allow-Origin to allow http://localhost:5500, but was '{allowedOrigin}'.");
    }

    [MonitoredFact]
    public void _05_OrderModel_HasOrderProcessedPropertyWithBindNever()
    {
        var property = typeof(Order).GetProperty("OrderProcessed");
        Assert.True(property != null,
            "The Order class must contain a property named 'OrderProcessed'.");

        Assert.True(property.PropertyType == typeof(bool),
            "The 'OrderProcessed' property on Order must be of type bool.");

        var bindNeverAttr = property.GetCustomAttributes(typeof(BindNeverAttribute), inherit: false).FirstOrDefault();
        Assert.True(bindNeverAttr != null,
            "The 'OrderProcessed' property on Order should be decorated with [BindNever] so client form binding cannot tamper with it.");
    }
}

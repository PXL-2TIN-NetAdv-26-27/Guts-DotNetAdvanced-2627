using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Guts.Client.XUnit;
using ToDoListApp.Domain;

namespace Guts.Tests;

[ExerciseTestClass("dotnet2", "10-ToDoListApp", "Api")]
public class ApiEndToEndTests : IAsyncLifetime
{
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient? _client;

    public ValueTask InitializeAsync()
    {
        _factory = new CustomWebApplicationFactory();
        try
        {
            _client = _factory.CreateClient();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        _client?.Dispose();
        await _factory.DisposeAsync();
    }

    [MonitoredFact]
    public async Task _01_GetById_ExistingList_ShouldReturn200WithListAndItems()
    {
        Guid listId = Guid.Empty;
        _factory.SeedData(db =>
        {
            var list = ToDoList.CreateNew("API Test List");
            list.Items.Add(ToDoItem.CreateNew("First item"));
            db.Set<ToDoList>().Add(list);
            listId = list.Id;
        });

        HttpResponseMessage response = await _client.GetAsync($"/api/todolists/{listId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string json = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(json);
        Assert.Equal("API Test List", doc.RootElement.GetProperty("title").GetString());
        Assert.Equal(1, doc.RootElement.GetProperty("items").GetArrayLength());
    }

    [MonitoredFact]
    public async Task _02_GetById_NonExistingList_ShouldReturn404()
    {
        HttpResponseMessage response = await _client.GetAsync($"/api/todolists/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [MonitoredFact]
    public async Task _03_GetById_NonGuidId_ShouldNotMatchAndReturn404()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/todolists/123");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [MonitoredFact]
    public async Task _04_Post_ValidModel_ShouldReturn201WithLocationHeaderAndCreatedList()
    {
        var payload = new { Title = "New API List" };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/todolists", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        string json = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(json);
        Guid createdId = doc.RootElement.GetProperty("id").GetGuid();
        Assert.NotEqual(Guid.Empty, createdId);
        Assert.Equal("New API List", doc.RootElement.GetProperty("title").GetString());
        Assert.Contains(createdId.ToString(), response.Headers.Location!.ToString(), StringComparison.OrdinalIgnoreCase);

        HttpResponseMessage followUp = await _client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, followUp.StatusCode);
    }

    [MonitoredFact]
    public async Task _05_Post_TitleTooShort_ShouldReturn400()
    {
        var payload = new { Title = "abc" };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/todolists", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [MonitoredFact]
    public async Task _06_Put_ExistingItem_ShouldReturn200WithUpdatedIsDone()
    {
        Guid listId = Guid.Empty;
        Guid itemId = Guid.Empty;
        _factory.SeedData(db =>
        {
            var list = ToDoList.CreateNew("Update Test List");
            var item = ToDoItem.CreateNew("Check item");
            list.Items.Add(item);
            db.Set<ToDoList>().Add(list);
            listId = list.Id;
            itemId = item.Id;
        });

        var payload = new { IsDone = true };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"/api/todolists/{listId}/items/{itemId}", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string json = await response.Content.ReadAsStringAsync();
        using JsonDocument doc = JsonDocument.Parse(json);
        Assert.True(doc.RootElement.GetProperty("isDone").GetBoolean());
        Assert.Equal("Check item", doc.RootElement.GetProperty("description").GetString());
    }

    [MonitoredFact]
    public async Task _07_Put_NonExistingListOrItem_ShouldReturn404()
    {
        var payload = new { IsDone = true };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"/api/todolists/{Guid.NewGuid()}/items/{Guid.NewGuid()}", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

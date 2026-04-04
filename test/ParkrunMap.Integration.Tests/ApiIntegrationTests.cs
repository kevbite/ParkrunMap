using Aspire.Hosting;
using Aspire.Hosting.Testing;

namespace ParkrunMap.Integration.Tests;

[CollectionDefinition("AspireIntegration", DisableParallelization = true)]
public sealed class AspireIntegrationCollection;

/// <summary>
/// Integration tests using Aspire's DistributedApplicationTestingBuilder to spin up
/// the full application stack (MongoDB, RabbitMQ, Api, Worker) and validate end-to-end behaviour.
/// These tests require Docker to be available.
/// </summary>
[Collection("AspireIntegration")]
public class ApiIntegrationTests : IAsyncLifetime
{
    private DistributedApplication? _app;
    private HttpClient? _client;

    public async Task InitializeAsync()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Program>();

        appHost.Services.ConfigureHttpClientDefaults(opts =>
            opts.AddStandardResilienceHandler());

        _app = await appHost.BuildAsync();
        await _app.StartAsync();

        _client = _app.CreateHttpClient("parkrun-api");
    }

    public async Task DisposeAsync()
    {
        if (_app is not null)
            await _app.DisposeAsync();
        _client?.Dispose();
    }

    [Fact]
    public async Task GetParkrunsByUk_ReturnsOk_WithEmptyDatabase()
    {
        var response = await _client!.GetAsync("/parkruns/uk");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Assert.NotNull(body);
        // On fresh database we expect an empty array
        Assert.Contains("[]", body);
    }

    [Fact]
    public async Task GetParkrunsByRegion_ReturnsOk_ForValidRegion()
    {
        var response = await _client!.GetAsync("/parkruns/region/London");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetParkrunsByRegion_ReturnsBadRequest_ForInvalidRegion()
    {
        var response = await _client!.GetAsync("/parkruns/region/InvalidRegion");

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetParkrunsByGeobox_ReturnsOk_WithValidBoundingBox()
    {
        var response = await _client!.GetAsync("/parkruns/geobox?lat=51.0&lat=52.0&lon=-0.5&lon=0.5");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetParkrunsByGeobox_ReturnsBadRequest_WithMissingParams()
    {
        var response = await _client!.GetAsync("/parkruns/geobox");

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}

using MassTransit;
using ParkrunMap.Api;
using ParkrunMap.Data.Mongo;
using ParkrunMap.Data.Mongo.Queries;
using ParkrunMap.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// OpenAPI
builder.Services.AddOpenApi();

// MongoDB
builder.Services.AddMongoClient(builder.Configuration, "parkrun-map");
builder.Services.AddParkrunMongo();

// MassTransit (for publishing messages from API if needed in future)
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        cfg.ConfigureEndpoints(ctx);
    });
});

builder.Services.AddCors(opts =>
    opts.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//  Parkrun API endpoints 

app.MapGet("/parkruns/uk", async (QueryParkrunsByRegion query, CancellationToken ct) =>
{
    var parkruns = await query.ExecuteAsync(ParkrunRegion.UK, ct);
    return Results.Ok(parkruns.Select(ParkrunResponse.From));
})
.WithName("GetParkrunsByUk")
.WithSummary("Get all UK parkruns")
.Produces<IEnumerable<ParkrunResponse>>();

app.MapGet("/parkruns/region/{region}", async (string region, QueryParkrunsByRegion query, CancellationToken ct) =>
{
    if (!Enum.TryParse<ParkrunRegion>(region, ignoreCase: true, out var regionEnum))
        return Results.BadRequest($"Unknown region '{region}'. Valid values: {string.Join(", ", Enum.GetNames<ParkrunRegion>())}");

    var parkruns = await query.ExecuteAsync(regionEnum, ct);
    return Results.Ok(parkruns.Select(ParkrunResponse.From));
})
.WithName("GetParkrunsByRegion")
.WithSummary("Get parkruns by region")
.Produces<IEnumerable<ParkrunResponse>>()
.ProducesProblem(400);

app.MapGet("/parkruns/geobox", async (
    [Microsoft.AspNetCore.Mvc.FromQuery] double[]? lat,
    [Microsoft.AspNetCore.Mvc.FromQuery] double[]? lon,
    QueryParkrunsByPolygon query,
    CancellationToken ct) =>
{
    if (lat is not { Length: 2 } || lon is not { Length: 2 })
        return Results.BadRequest("Provide exactly two lat and two lon values defining the bounding box corners");

    var minLon = Math.Min(lon[0], lon[1]);
    var maxLon = Math.Max(lon[0], lon[1]);
    var minLat = Math.Min(lat[0], lat[1]);
    var maxLat = Math.Max(lat[0], lat[1]);

    // GeoJSON polygon [lon, lat] order
    double[][] polygon =
    [
        [minLon, minLat],
        [maxLon, minLat],
        [maxLon, maxLat],
        [minLon, maxLat],
        [minLon, minLat]
    ];

    var parkruns = await query.ExecuteAsync(polygon, ct);
    return Results.Ok(parkruns.Select(ParkrunResponse.From));
})
.WithName("GetParkrunsByGeoBox")
.WithSummary("Get parkruns within a geographic bounding box")
.Produces<IEnumerable<ParkrunResponse>>()
.ProducesProblem(400);

app.Run();

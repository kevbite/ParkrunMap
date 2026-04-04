using System.Text.Json;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using MongoDB.Driver;
using ParkrunMap.Domain;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ParkrunMap.Integration.Tests;

[Collection("AspireIntegration")]
public class EventsJsonEndToEndTests : IAsyncLifetime
{
    private WireMockServer? _wireMock;
    private DistributedApplication? _app;
    private HttpClient? _client;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    //  Response DTOs (mirror the camelCase JSON the API returns) 

    private sealed record ParkrunApiResponse(
        string Name,
        string Uri,
        double Lat,
        double Lon,
        List<CancellationApiResponse> Cancellations,
        CourseApiResponse Course,
        FeaturesApiResponse Features,
        SpecialEventsApiResponse SpecialEvents);

    private sealed record CancellationApiResponse(DateTime Date, string? Reason);

    private sealed record CourseApiResponse(
        string? Description,
        List<string> GoogleMapIds,
        List<int> Terrain);

    private sealed record FeaturesApiResponse(
        bool? WheelchairFriendly, bool? BuggyFriendly, bool? VisuallyImpairedFriendly,
        bool? Toilets, bool? DogsAllowed, bool? Cafe, bool? PostRunCoffee,
        bool? DrinkingFountain, bool? ChangingRooms, bool? Lockers, bool? Showers,
        bool? BagDrop, bool? BabyChangingFacilities, bool? CarParking, bool? CycleParking,
        List<int> CarParkingOptions, List<int> CycleParkingOptions, List<int> RecommendedBuggy);

    private sealed record SpecialEventsApiResponse(
        List<int> ChristmasDay,
        List<int> NewYearsDay);

    //  Expected values derived directly from the sample payload ─

    private sealed record ExpectedParkrun(string Name, string Uri, double Lat, double Lon);

    private static readonly ExpectedParkrun[] ExpectedUkParkruns =
    [
        new("Bushy parkrun",  "https://www.parkrun.org.uk/bushy/",  51.410992, -0.335791),
        new("Dudley parkrun", "https://www.parkrun.org.uk/dudley/", 52.490027, -2.134568),
        new("Antrim parkrun", "https://www.parkrun.org.uk/antrim/", 54.717078, -6.238818),
    ];

    private static readonly string SampleEventsJson = """
        {
          "events": {
            "features": [
              { "id": 1,    "type": "Feature", "geometry": { "coordinates": [-0.335791, 51.410992] }, "properties": { "eventname": "bushy",             "EventLongName": "Bushy parkrun",                "EventShortName": "Bushy Park",       "countrycode": 97, "seriesid": 1, "EventLocation": "Bushy Park" } },
              { "id": 111,  "type": "Feature", "geometry": { "coordinates": [-2.134568, 52.490027] }, "properties": { "eventname": "dudley",            "EventLongName": "Dudley parkrun",               "EventShortName": "Dudley",           "countrycode": 97, "seriesid": 1, "EventLocation": "The Dell Stadium" } },
              { "id": 585,  "type": "Feature", "geometry": { "coordinates": [-6.238818, 54.717078] }, "properties": { "eventname": "antrim",            "EventLongName": "Antrim parkrun",               "EventShortName": "Antrim",           "countrycode": 97, "seriesid": 1, "EventLocation": "Loughshore Park" } },
              { "id": 32,   "type": "Feature", "geometry": { "coordinates": [-0.080886, 51.365750] }, "properties": { "eventname": "lloyd-juniors",     "EventLongName": "Lloyd junior parkrun",         "EventShortName": "Lloyd juniors",    "countrycode": 97, "seriesid": 2, "EventLocation": "Lloyd Park" } },
              { "id": 25,   "type": "Feature", "geometry": { "coordinates": [151.18,   -33.91]     }, "properties": { "eventname": "stpeters",          "EventLongName": "St Peters parkrun",            "EventShortName": "St Peters",        "countrycode": 3,  "seriesid": 1, "EventLocation": "St Peters" } },
              { "id": 2857, "type": "Feature", "geometry": { "coordinates": [13.07,     47.76]     }, "properties": { "eventname": "hellbrunn",         "EventLongName": "Hellbrunn parkrun",            "EventShortName": "Hellbrunn",        "countrycode": 4,  "seriesid": 1, "EventLocation": "Hellbrunn" } },
              { "id": 1517, "type": "Feature", "geometry": { "coordinates": [-114.13,   51.13]     }, "properties": { "eventname": "nosehill",          "EventLongName": "Nose Hill parkrun",            "EventShortName": "Nose Hill",        "countrycode": 14, "seriesid": 1, "EventLocation": "Nose Hill" } },
              { "id": 13,   "type": "Feature", "geometry": { "coordinates": [12.58,     55.65]     }, "properties": { "eventname": "amager",            "EventLongName": "Amager Faelled parkrun",       "EventShortName": "Amager",           "countrycode": 23, "seriesid": 1, "EventLocation": "Amager" } },
              { "id": 1753, "type": "Feature", "geometry": { "coordinates": [23.76,     61.49]     }, "properties": { "eventname": "tampere",           "EventLongName": "Tampere parkrun",              "EventShortName": "Tampere",          "countrycode": 30, "seriesid": 1, "EventLocation": "Tampere" } },
              { "id": 1774, "type": "Feature", "geometry": { "coordinates": [8.45,      49.45]     }, "properties": { "eventname": "neckarau",          "EventLongName": "Neckarau parkrun",             "EventShortName": "Neckarau",         "countrycode": 32, "seriesid": 1, "EventLocation": "Neckarau" } },
              { "id": 178,  "type": "Feature", "geometry": { "coordinates": [-6.69,     53.66]     }, "properties": { "eventname": "navan",             "EventLongName": "Navan parkrun",                "EventShortName": "Navan",            "countrycode": 42, "seriesid": 1, "EventLocation": "Navan" } },
              { "id": 1239, "type": "Feature", "geometry": { "coordinates": [13.33,     38.13]     }, "properties": { "eventname": "uditore",           "EventLongName": "Uditore parkrun",              "EventShortName": "Uditore",          "countrycode": 44, "seriesid": 1, "EventLocation": "Uditore" } },
              { "id": 2295, "type": "Feature", "geometry": { "coordinates": [139.62,    35.61]     }, "properties": { "eventname": "futakotamagawa",   "EventLongName": "Futakotamagawa parkrun",       "EventShortName": "Futakotamagawa",   "countrycode": 46, "seriesid": 1, "EventLocation": "Futakotamagawa" } },
              { "id": 3549, "type": "Feature", "geometry": { "coordinates": [25.25,     54.68]     }, "properties": { "eventname": "vingis",            "EventLongName": "parkrun Vingis",               "EventShortName": "Vingis",           "countrycode": 54, "seriesid": 1, "EventLocation": "Vingis" } },
              { "id": 2170, "type": "Feature", "geometry": { "coordinates": [101.70,     2.92]     }, "properties": { "eventname": "presint18",         "EventLongName": "Presint 18 parkrun",           "EventShortName": "Presint 18",       "countrycode": 57, "seriesid": 1, "EventLocation": "Putrajaya" } },
              { "id": 2603, "type": "Feature", "geometry": { "coordinates": [6.54,      53.20]     }, "properties": { "eventname": "stadspark",         "EventLongName": "Stadspark parkrun Groningen",  "EventShortName": "Stadspark",        "countrycode": 64, "seriesid": 1, "EventLocation": "Groningen" } },
              { "id": 354,  "type": "Feature", "geometry": { "coordinates": [174.90,   -41.21]     }, "properties": { "eventname": "lowerhutt",         "EventLongName": "Lower Hutt parkrun",           "EventShortName": "Lower Hutt",       "countrycode": 65, "seriesid": 1, "EventLocation": "Lower Hutt" } },
              { "id": 1734, "type": "Feature", "geometry": { "coordinates": [10.78,     59.92]     }, "properties": { "eventname": "toyen",             "EventLongName": "Toeyen parkrun",               "EventShortName": "Toeyen",           "countrycode": 67, "seriesid": 1, "EventLocation": "Toeyen" } },
              { "id": 337,  "type": "Feature", "geometry": { "coordinates": [18.56,     54.50]     }, "properties": { "eventname": "gdynia",            "EventLongName": "parkrun Gdynia",               "EventShortName": "Gdynia",           "countrycode": 74, "seriesid": 1, "EventLocation": "Gdynia" } },
              { "id": 915,  "type": "Feature", "geometry": { "coordinates": [103.89,     1.29]     }, "properties": { "eventname": "eastcoastpark",    "EventLongName": "East Coast Park parkrun",      "EventShortName": "East Coast Park",  "countrycode": 82, "seriesid": 1, "EventLocation": "East Coast Park" } },
              { "id": 303,  "type": "Feature", "geometry": { "coordinates": [22.23,    -33.53]     }, "properties": { "eventname": "oudtshoorn",        "EventLongName": "Oudtshoorn parkrun",           "EventShortName": "Oudtshoorn",       "countrycode": 85, "seriesid": 1, "EventLocation": "Oudtshoorn" } },
              { "id": 1527, "type": "Feature", "geometry": { "coordinates": [18.04,     59.35]     }, "properties": { "eventname": "haga",              "EventLongName": "Haga parkrun",                 "EventShortName": "Haga",             "countrycode": 88, "seriesid": 1, "EventLocation": "Haga" } },
              { "id": 166,  "type": "Feature", "geometry": { "coordinates": [-77.06,    38.90]     }, "properties": { "eventname": "rooseveltislanddc", "EventLongName": "Roosevelt Island DC parkrun",  "EventShortName": "Roosevelt Island", "countrycode": 98, "seriesid": 1, "EventLocation": "Roosevelt Island" } }
            ]
          },
          "countries": {
            "3":  { "url": "www.parkrun.com.au", "cc": "AU", "name": "Australia" },
            "4":  { "url": "www.parkrun.at",     "cc": "AT", "name": "Austria" },
            "14": { "url": "www.parkrun.ca",     "cc": "CA", "name": "Canada" },
            "23": { "url": "www.parkrun.dk",     "cc": "DK", "name": "Denmark" },
            "30": { "url": "www.parkrun.fi",     "cc": "FI", "name": "Finland" },
            "32": { "url": "www.parkrun.de",     "cc": "DE", "name": "Germany" },
            "42": { "url": "www.parkrun.ie",     "cc": "IE", "name": "Ireland" },
            "44": { "url": "www.parkrun.it",     "cc": "IT", "name": "Italy" },
            "46": { "url": "www.parkrun.jp",     "cc": "JP", "name": "Japan" },
            "54": { "url": "www.parkrun.lt",     "cc": "LT", "name": "Lithuania" },
            "57": { "url": "www.parkrun.my",     "cc": "MY", "name": "Malaysia" },
            "64": { "url": "www.parkrun.nl",     "cc": "NL", "name": "Netherlands" },
            "65": { "url": "www.parkrun.co.nz",  "cc": "NZ", "name": "New Zealand" },
            "67": { "url": "www.parkrun.no",     "cc": "NO", "name": "Norway" },
            "74": { "url": "www.parkrun.pl",     "cc": "PL", "name": "Poland" },
            "82": { "url": "www.parkrun.sg",     "cc": "SG", "name": "Singapore" },
            "85": { "url": "www.parkrun.co.za",  "cc": "ZA", "name": "South Africa" },
            "88": { "url": "www.parkrun.se",     "cc": "SE", "name": "Sweden" },
            "97": { "url": "www.parkrun.org.uk", "cc": "UK", "name": "United Kingdom" },
            "98": { "url": "www.parkrun.us",     "cc": "US", "name": "United States" }
          }
        }
        """;

    public async Task InitializeAsync()
    {
        _wireMock = WireMockServer.Start();

        _wireMock
            .Given(Request.Create().WithPath("/events.json").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody(SampleEventsJson));

        Environment.SetEnvironmentVariable("Scraping__EventsJsonBaseUrl", _wireMock.Urls[0]);
        Environment.SetEnvironmentVariable("Jobs__RunAtStartup", "true");

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
        _wireMock?.Stop();
        _wireMock?.Dispose();

        Environment.SetEnvironmentVariable("Scraping__EventsJsonBaseUrl", null);
        Environment.SetEnvironmentVariable("Jobs__RunAtStartup", null);
    }

    //  Helper: wait until all 3 UK parkruns are visible via the API 

    private async Task WaitForUkParkrunsAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var response = await _client!.GetAsync("/parkruns/uk", ct);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync(ct);
            var parkruns = JsonSerializer.Deserialize<List<ParkrunApiResponse>>(body, JsonOptions);

            if (parkruns is not null &&
                ExpectedUkParkruns.All(e => parkruns.Any(p => p.Name == e.Name)))
                return;

            await Task.Delay(500, ct);
        }
    }

    [Fact]
    public async Task AllParkrunEvents_AreStoredAndQueryable_AfterEventsJsonDownload()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        await WaitForUkParkrunsAsync(cts.Token);

        var response = await _client!.GetAsync("/parkruns/uk", cts.Token);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync(cts.Token);
        var parkruns = JsonSerializer.Deserialize<List<ParkrunApiResponse>>(body, JsonOptions)!;

        foreach (var expected in ExpectedUkParkruns)
        {
            var actual = Assert.Single(parkruns, p => p.Name == expected.Name);

            // Core mapping from events.json: name, website URL, and coordinates.
            Assert.Equal(expected.Uri, actual.Uri);
            Assert.Equal(expected.Lat, actual.Lat, precision: 4);
            Assert.Equal(expected.Lon, actual.Lon, precision: 4);

            // UpsertParkrun only sets identity/location fields; enrichment fields
            // (course, features, cancellations, special events) must all be empty
            // immediately after the events.json pipeline has run.
            Assert.Empty(actual.Cancellations);
            Assert.Null(actual.Features.WheelchairFriendly);
            Assert.Null(actual.Features.BuggyFriendly);
            Assert.Empty(actual.SpecialEvents.ChristmasDay);
            Assert.Empty(actual.SpecialEvents.NewYearsDay);
        }
    }

    [Fact]
    public async Task JuniorParkruns_AreNotStoredInDatabase()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        await WaitForUkParkrunsAsync(cts.Token);

        var response = await _client!.GetAsync("/parkruns/uk", cts.Token);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync(cts.Token);
        var parkruns = JsonSerializer.Deserialize<List<ParkrunApiResponse>>(body, JsonOptions)!;

        Assert.DoesNotContain(parkruns, p => p.Name == "Lloyd junior parkrun");
    }

    //  Expected MongoDB document values after the events.json pipeline 

    private sealed record ExpectedMongoDoc(
        int GeoXmlId, string Name, string Domain, string Path, double Lat, double Lon,
        string Country);

    private static readonly ExpectedMongoDoc[] ExpectedUkMongoDocs =
    [
        new(1,   "Bushy parkrun",  "www.parkrun.org.uk", "/bushy/",  51.410992, -0.335791, "UK"),
        new(111, "Dudley parkrun", "www.parkrun.org.uk", "/dudley/", 52.490027, -2.134568, "UK"),
        new(585, "Antrim parkrun", "www.parkrun.org.uk", "/antrim/", 54.717078, -6.238818, "UK"),
    ];

    [Fact]
    public async Task ParkrunEvents_AreStoredWithCorrectFields_InMongoDb()
    {
        // Wait until the events are visible via the API.
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        await WaitForUkParkrunsAsync(cts.Token);

        // Connect directly to MongoDB via the Aspire-managed connection string.
        var connectionString = await _app!.GetConnectionStringAsync("mongodb");
        var mongoClient = new MongoClient(connectionString);
        var collection = mongoClient
            .GetDatabase("parkrun-map")
            .GetCollection<Parkrun>("parkruns");

        foreach (var expected in ExpectedUkMongoDocs)
        {
            var doc = await collection
                .Find(Builders<Parkrun>.Filter.Eq(p => p.GeoXmlId, expected.GeoXmlId))
                .FirstOrDefaultAsync();

            Assert.NotNull(doc);

            // Identity and website fields set by the events.json pipeline.
            Assert.Equal(expected.Name, doc.Name);
            Assert.Equal(expected.Domain, doc.Website.Domain);
            Assert.Equal(expected.Path, doc.Website.Path);

            // Country comes from the "countries" section of events.json; Region is not
            // present in events.json so it is stored as empty string.
            Assert.Equal(string.Empty, doc.Region);
            Assert.Equal(expected.Country, doc.Country);

            // GeoJSON point stored as [longitude, latitude].
            Assert.Equal(expected.Lon, doc.Location.Coordinates.Longitude, precision: 4);
            Assert.Equal(expected.Lat, doc.Location.Coordinates.Latitude, precision: 4);
        }
    }

    [Fact]
    public async Task ParkrunCourses_AreScrapedAndStored_InMongoDb()
    {
        // Connect directly to MongoDB via the Aspire-managed connection string.
        var connectionString = await _app!.GetConnectionStringAsync("mongodb");
        var mongoClient = new MongoClient(connectionString);
        var collection = mongoClient
            .GetDatabase("parkrun-map")
            .GetCollection<Parkrun>("parkruns");

        // Poll until all 3 UK parkruns have a non-empty course description.
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));
        while (!cts.IsCancellationRequested)
        {
            var docs = await collection
                .Find(Builders<Parkrun>.Filter.In(
                    p => p.GeoXmlId, ExpectedUkMongoDocs.Select(e => e.GeoXmlId)))
                .ToListAsync(cts.Token);

            if (docs.Count == ExpectedUkMongoDocs.Length &&
                docs.All(d => !string.IsNullOrWhiteSpace(d.Course.Description)))
                break;

            await Task.Delay(1000, cts.Token);
        }

        cts.Token.ThrowIfCancellationRequested();

        foreach (var expected in ExpectedUkMongoDocs)
        {
            var doc = await collection
                .Find(Builders<Parkrun>.Filter.Eq(p => p.GeoXmlId, expected.GeoXmlId))
                .FirstOrDefaultAsync();

            Assert.NotNull(doc);
            Assert.False(string.IsNullOrWhiteSpace(doc.Course.Description),
                $"{expected.Name}: Course.Description should not be empty after scraping");
        }
    }
}
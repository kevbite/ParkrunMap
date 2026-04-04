# ParkrunMap2 — Migration Plan

## Overview

Rewrite the ParkrunMap Azure Functions application as a modern ASP.NET API using:

- **ASP.NET Core** (Minimal APIs or Controllers) for HTTP endpoints
- **.NET Aspire** for orchestration, service defaults, and local dev experience
- **MassTransit + RabbitMQ** for messaging (replacing Azure Storage Queues + Blob triggers)
- **Hangfire + MongoDB** for job scheduling (replacing Azure Functions timer triggers)
- **Playwright** for web scraping (replacing raw HttpClient + HtmlAgilityPack)
- **MongoDB** (existing database, same schema)

---

## Current Architecture (Azure Functions)

```
Timer Triggers ──► Download HTML/JSON ──► Blob Storage
                                              │
                                       Blob Triggers
                                              │
                                     Parse / Extract
                                              │
                                       Queue Messages
                                              │
                                    Queue Triggers ──► MongoDB
                                              
HTTP Triggers ──► MediatR ──► MongoDB ──► JSON Response
```

### Existing Data Pipelines

| Pipeline              | Schedule             | Flow                                                        |
|-----------------------|----------------------|-------------------------------------------------------------|
| Parkrun Events        | Monday 2 AM          | Download events.json → parse → upsert parkruns to MongoDB   |
| Course Details        | Every hour 8AM–11PM  | Pick unprocessed parkrun → download course page → parse → update MongoDB |
| Cancellations         | Every 2 hours        | Download cancellations HTML → parse → update MongoDB        |
| Questionnaire         | Monday 2 AM          | Download Google Sheet → aggregate → update features in MongoDB |
| Special Events        | 1–7 Dec, 7:15 AM    | Download special events pages (5 countries) → parse → update MongoDB |

### Existing API Endpoints

| Route                            | Method | Description                          |
|----------------------------------|--------|--------------------------------------|
| `/parkruns/uk`                   | GET    | Query all UK parkruns                |
| `/parkruns/region/{region}`      | GET    | Query parkruns by region             |
| `/parkruns/geobox`              | GET    | Query parkruns by geographic bounding box |

---

## Target Architecture (ASP.NET + Aspire)

```
┌──────────────────────────────────────────────────────────────┐
│                      .NET Aspire AppHost                      │
│  Orchestrates: API, Worker, MongoDB, RabbitMQ, Playwright     │
└──────────────────────────────────────────────────────────────┘
         │              │              │             │
    ┌────┴────┐   ┌─────┴─────┐   ┌──┴───┐   ┌────┴─────┐
    │   API   │   │  Worker   │   │ Mongo │   │ RabbitMQ │
    │  Host   │   │  Service  │   │  DB   │   │          │
    └────┬────┘   └─────┬─────┘   └──┬───┘   └────┬─────┘
         │              │             │             │
    HTTP Endpoints  Hangfire Jobs     │        MassTransit
    (ASP.NET Core)  + Consumers      │        Messages
         │              │             │             │
         └──────────────┴─────────────┴─────────────┘
```

### Projects

| Project                                | Type               | Purpose                                                         |
|----------------------------------------|--------------------|-----------------------------------------------------------------|
| `ParkrunMap.AppHost`                   | Aspire AppHost     | Orchestrates all services, MongoDB, RabbitMQ                    |
| `ParkrunMap.ServiceDefaults`           | Aspire Defaults    | Shared config: health checks, OpenTelemetry, resilience         |
| `ParkrunMap.Api`                       | ASP.NET Core API   | HTTP endpoints, Hangfire dashboard                              |
| `ParkrunMap.Worker`                    | Worker Service     | MassTransit consumers, Hangfire job server, Playwright scraping |
| `ParkrunMap.Domain`                    | Class Library      | Domain entities (largely reused from existing)                  |
| `ParkrunMap.Data.Mongo`                | Class Library      | MongoDB repositories / data access (evolved from existing)      |
| `ParkrunMap.Scraping`                  | Class Library      | Playwright-based scrapers + parsers                             |
| `ParkrunMap.Contracts`                 | Class Library      | MassTransit message contracts (shared between API & Worker)     |

---

## Step-by-Step Implementation Plan

### Phase 1: Solution Scaffolding & Infrastructure

#### 1.1 — Create .NET Aspire Solution
- Create a new .NET 9 Aspire solution (`ParkrunMap2.sln`)
- Scaffold `ParkrunMap.AppHost` and `ParkrunMap.ServiceDefaults` projects
- Add `ParkrunMap.Api` (ASP.NET Core Web API)
- Add `ParkrunMap.Worker` (Worker Service)
- Add class libraries: `Domain`, `Data.Mongo`, `Scraping`, `Contracts`
- Set up `global.json` pinning .NET 9 SDK

#### 1.2 — Configure Aspire AppHost Resources
- Add MongoDB resource (use connection string for existing database)
- Add RabbitMQ resource (container for local dev, connection string for prod)
- Wire up service references between projects
- Configure service defaults (health checks, OpenTelemetry, resilience)

#### 1.3 — Configure Docker Compose / Aspire Containers
- RabbitMQ container with management UI
- MongoDB container (for local dev; prod uses existing instance)
- Ensure Aspire dashboard is working for local development

---

### Phase 2: Domain & Data Layer

#### 2.1 — Migrate Domain Entities
- Copy/adapt domain models from existing `ParkrunMap.Domain`
  - `Parkrun`, `Course`, `Features`, `Cancellation`, `Statistics`, `SpecialEvents`
  - `Website` (Domain + Path)
  - `GeoJSON` location types
- Modernize to use records/init properties where appropriate
- Keep MongoDB serialization compatibility with existing data

#### 2.2 — Migrate MongoDB Data Access
- Set up MongoDB driver configuration via Aspire's MongoDB integration
- Migrate repository logic from existing MediatR handlers to repository classes:
  - `ParkrunRepository` with methods:
    - `UpsertParkrunAsync()`
    - `QueryByPolygonAsync()` (geospatial)
    - `QueryByRegionAsync()`
    - `QueryFirstUnprocessedAsync()`
    - `AddCancellationAsync()`
    - `UpdateFeaturesAsync()`
    - `UpdateStatisticsAsync()`
    - `UpdateSpecialEventsAsync()`
    - `UpdateCourseDetailsAsync()`
- Reuse existing MongoDB aggregation logic (geo queries, 3-week cancellation filter)
- Ensure indexes are maintained (geospatial on Location field)

#### 2.3 — Region Polygon Provider
- Migrate `UkRegionPolygonProvider` and region polygon data
- Support for `ParkrunRegion` enum

---

### Phase 3: MassTransit Messaging

#### 3.1 — Define Message Contracts
- Create message contracts in `ParkrunMap.Contracts`:
  - `UpsertParkrun` — parkrun master data from events.json
  - `ScrapeCourseBatch` — **batch** of parkrun URLs to scrape in a single browser session (list of website domain+path pairs)
  - `UpdateCourseDetails` — parsed course data to store (one per parkrun, published after batch completes)
  - `UpdateParkrunStatistics` — parsed statistics to store (one per parkrun)
  - `AddParkrunCancellation` — cancellation to add
  - `UpdateParkrunFeatures` — aggregated features to store
  - `ScrapeSpecialEventsBatch` — **batch** of country site URLs to scrape in a single browser session
  - `UpdateParkrunSpecialEvents` — parsed special events to store
  - `AggregateQuestionnaireResponses` — questionnaire batch to aggregate

#### 3.2 — Configure MassTransit + RabbitMQ
- Install MassTransit.RabbitMQ in Worker project
- Configure MassTransit in both API and Worker via Aspire
- Set up message retry policies, error queues, dead letter handling
- Configure concurrency limits (replace the Azure Functions `batchSize: 4`)

#### 3.3 — Implement MassTransit Consumers (Worker)
- Create consumers for each message type:
  - `UpsertParkrunConsumer` → calls repository to upsert
  - `ScrapeCourseBatchConsumer` → receives a batch of parkrun URLs → opens **one Playwright browser session** → navigates to each course page sequentially within that session → extracts data → publishes individual `UpdateCourseDetails` + `UpdateParkrunStatistics` messages per parkrun
  - `UpdateCourseDetailsConsumer` → calls repository
  - `UpdateParkrunStatisticsConsumer` → calls repository
  - `AddParkrunCancellationConsumer` → calls repository
  - `UpdateParkrunFeaturesConsumer` → calls repository
  - `ScrapeSpecialEventsBatchConsumer` → receives batch of country URLs → opens **one Playwright browser session** → navigates each country page → publishes individual `UpdateParkrunSpecialEvents` messages
  - `UpdateParkrunSpecialEventsConsumer` → calls repository
  - `AggregateQuestionnaireResponsesConsumer` → aggregates → publishes feature updates
- **Batch consumer pattern**: Consumers that use Playwright accept a list of URLs, spin up a single browser context, iterate through pages collecting data, then publish individual update messages for each result. This amortizes browser startup cost across many pages.

---

### Phase 4: Playwright Scraping

#### 4.1 — Set Up Playwright Infrastructure
- Add `Microsoft.Playwright` NuGet package to `ParkrunMap.Scraping`
- Create a `PlaywrightBrowserFactory` / `IBrowserService` for managed browser lifecycle
- **Single browser instance, reused across a batch of pages** — a batch consumer creates one `IBrowserContext`, navigates to each URL in the batch sequentially, extracts data, then disposes the context
- The browser process itself (`IPlaywright` + `IBrowser`) is a long-lived singleton managed by DI
- New pages are created per URL within the batch, then closed after extraction — keeps memory bounded
- Register as singleton in DI with proper disposal

#### 4.2 — Migrate Scrapers to Playwright
- **EventsJsonScraper**: Download `https://images.parkrun.com/events.json`
  - This is a plain JSON download — could remain as simple HttpClient or use Playwright
  - Parse with existing `EventsJsonParser` logic (adapted)
- **CoursePageScraper**: Accepts a **batch of parkrun URLs** (`List<Website>`)
  - Opens one browser context, creates a new page per URL, extracts data, closes page
  - Returns a list of results (course description, Google Map iframes, terrain, statistics per parkrun)
  - Handles individual page failures gracefully (log + skip, don't fail the whole batch)
- **CancellationsScraper**: Navigate to `https://www.parkrun.org.uk/cancellations/`
  - Use Playwright to render the cancellations table
  - Replace `CancellationsParser` with Playwright-based extraction
- **SpecialEventsScraper**: Accepts a **batch of country site URLs**
  - One browser context, iterates through each country page, extracts running status per year
  - Replace `SpecialEventsParser`
- **QuestionnaireScraper**: Download from Google Sheets API
  - This is a REST API call — can remain HttpClient-based
  - Adapt `QuestionnaireResponseDownloader` logic

#### 4.3 — Remove Proxy Rotation
- Playwright handles its own network stack
- Evaluate whether proxies are still needed (Playwright may solve the blocking issues)
- If still needed, configure Playwright browser proxy settings instead

---

### Phase 5: Hangfire Scheduling

#### 5.1 — Set Up Hangfire with MongoDB
- Install `Hangfire.Core` and `Hangfire.Mongo` packages
- Configure Hangfire to use the existing MongoDB database
- Mount Hangfire dashboard in the API project (at `/hangfire`)
- Secure the dashboard (authentication/authorization)

#### 5.2 — Create Scheduled Jobs
- Replace Azure Functions timer triggers with Hangfire recurring jobs:

| Job Name                    | Cron Schedule           | Action                                                      |
|-----------------------------|-------------------------|-------------------------------------------------------------|
| `DownloadEventsJson`        | `0 2 * * MON`          | Download events.json → parse → publish `UpsertParkrun` messages |
| `DownloadCancellationsPage` | `0 */2 * * *`          | Download cancellations → parse → publish `AddParkrunCancellation` messages |
| `QueueCourseDownloads`      | `0 8-23 * * *`         | Query next **batch** of unprocessed parkruns (e.g. 20) → publish single `ScrapeCourseBatch` message |
| `DownloadQuestionnaire`     | `0 2 * * MON`          | Download Google Sheet → publish `AggregateQuestionnaireResponses` messages |
| `QueueSpecialEventsDownload`| `15 7 1-7 12 *`        | Publish single `ScrapeSpecialEventsBatch` with all 5 country site URLs |

#### 5.3 — Job Registration
- Register recurring jobs on application startup
- Implement `IRecurringJobManager` integration
- Add job retry policies and failure logging

---

### Phase 6: ASP.NET Core API

#### 6.1 — Implement API Endpoints
- Create Minimal API endpoints (or Controllers) matching existing routes:
  - `GET /parkruns/uk` → query MongoDB for UK region parkruns
  - `GET /parkruns/region/{region}` → query by region polygon
  - `GET /parkruns/geobox?lat=...&lon=...` → query by bounding box
- Wire up to repository layer
- Add response DTOs matching existing JSON shape for backward compatibility
- Add AutoMapper or manual mapping for Domain → DTO

#### 6.2 — Add API Features
- CORS configuration (for frontend consumers)
- Response caching (parkrun data doesn't change frequently)
- Health check endpoints (MongoDB, RabbitMQ connectivity)
- OpenAPI/Swagger documentation
- Rate limiting if needed

---

### Phase 7: Testing

#### 7.1 — Unit Tests
- Parser tests (migrate existing `ParkrunMap.Scraping.Tests`)
- Repository tests (migrate existing `ParkrunMap.Data.Mongo.Tests`)
- Consumer tests (MassTransit test harness)
- API endpoint tests

#### 7.2 — Integration Tests
- Use Aspire's `DistributedApplicationTestingBuilder` for integration tests
- Test full message flow: publish message → consumer processes → MongoDB updated
- Test API endpoints against real MongoDB (via Aspire test containers)
- Playwright scraper integration tests (against test HTML fixtures)

---

### Phase 8: CI/CD & Deployment

#### 8.1 — Containerization
- Add Dockerfiles for API and Worker services
- **Worker Dockerfile must support Playwright**:
  - Use `mcr.microsoft.com/playwright/dotnet` as the runtime base image (includes Chromium, Firefox, WebKit and all OS-level dependencies)
  - Alternatively, use a standard .NET runtime image and run `pwsh bin/Debug/netX.0/playwright.ps1 install --with-deps chromium` at build time to install browser + system libs
  - Ensure the container runs as non-root where possible (Playwright supports `--ipc=host` or `--security-opt seccomp=unconfined` if sandboxing issues arise)
  - Set `PLAYWRIGHT_BROWSERS_PATH=/ms-playwright` environment variable to control browser cache location
  - Keep the API Dockerfile lightweight (no Playwright/browser dependencies needed)
- Configure Aspire for container publishing
- Set up docker-compose for production deployment
  - Worker container may need `--shm-size=1gb` (or `/dev/shm` mount) to prevent Chromium crashes in low-memory shared memory environments

#### 8.2 — CI Pipeline
- Replace `azure-pipelines.yml` with GitHub Actions (or update for new structure)
- Build, test, publish containers
- Playwright browser installation in CI

#### 8.3 — Production Configuration
- MongoDB connection string (existing database)
- RabbitMQ connection string (new instance)
- Hangfire MongoDB storage configuration
- Environment-specific Aspire configuration

---

## Key Design Decisions

### Why MassTransit over raw RabbitMQ?
- Provides consumer abstractions, retry policies, error handling, saga support
- Message serialization, routing, and queue topology management
- Built-in test harness for unit testing consumers
- Easy swap between transports if needed later

### Why Hangfire over Quartz.NET?
- MongoDB backend support out of the box (`Hangfire.Mongo`)
- Dashboard UI for monitoring and manual triggering
- Simpler API for recurring jobs
- Good integration with ASP.NET Core DI

### Why Playwright over HttpClient + HtmlAgilityPack?
- Handles JavaScript-rendered content (Parkrun site likely uses JS frameworks now)
- Built-in browser automation handles cookies, redirects, and anti-bot measures
- No need for manual proxy rotation (browser context isolation)
- More resilient to site changes

### Why .NET Aspire?
- First-class local development experience (containers, service discovery)
- Built-in health checks, telemetry, and resilience
- Simplified service-to-service communication
- Dashboard for monitoring during development

### Backward Compatibility
- Same MongoDB database and collection structure
- Same API response JSON shape
- Existing consumers of the API should not need changes

---

## Risk & Considerations

1. **Playwright resource usage**: Mitigated by batching — a single browser session processes many pages before shutting down. Singleton `IBrowser` avoids repeated startup. Configure batch sizes to balance memory vs throughput.
2. **MongoDB schema migration**: No schema changes planned, but verify serialization compatibility.
3. **RabbitMQ durability**: Ensure queues are durable and messages are persistent.
4. **Hangfire MongoDB storage**: Verify Hangfire.Mongo works well with the existing MongoDB version.
5. **Rate limiting on Parkrun sites**: Playwright may trigger different anti-bot measures than HttpClient did.
6. **Google Sheets API**: May need to update authentication method if API keys are deprecated.

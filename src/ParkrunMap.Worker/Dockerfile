# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/ParkrunMap.Worker/ParkrunMap.Worker.csproj", "src/ParkrunMap.Worker/"]
COPY ["src/ParkrunMap.ServiceDefaults/ParkrunMap.ServiceDefaults.csproj", "src/ParkrunMap.ServiceDefaults/"]
COPY ["src/ParkrunMap.Domain/ParkrunMap.Domain.csproj", "src/ParkrunMap.Domain/"]
COPY ["src/ParkrunMap.Contracts/ParkrunMap.Contracts.csproj", "src/ParkrunMap.Contracts/"]
COPY ["src/ParkrunMap.Data.Mongo/ParkrunMap.Data.Mongo.csproj", "src/ParkrunMap.Data.Mongo/"]
COPY ["src/ParkrunMap.Scraping/ParkrunMap.Scraping.csproj", "src/ParkrunMap.Scraping/"]

RUN dotnet restore "src/ParkrunMap.Worker/ParkrunMap.Worker.csproj"

COPY src/ src/

WORKDIR "/src/src/ParkrunMap.Worker"
RUN dotnet publish "ParkrunMap.Worker.csproj" -c Release -o /app/publish --no-restore

# Runtime stage — use the Playwright .NET image so browsers are available
# https://playwright.dev/dotnet/docs/docker
# Version matches the Microsoft.Playwright NuGet package (1.58.0)
FROM mcr.microsoft.com/playwright/dotnet:v1.58.0-noble AS final
WORKDIR /app

COPY --from=build /app/publish .

# The Playwright base image already has browsers installed at /ms-playwright.
# The version tag matches the Microsoft.Playwright NuGet package (1.58.0), so no
# additional browser installation is needed.

# Run as the pwuser provided by the Playwright base image (non-root)
USER pwuser

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "ParkrunMap.Worker.dll"]

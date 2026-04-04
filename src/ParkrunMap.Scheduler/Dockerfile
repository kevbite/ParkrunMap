# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/ParkrunMap.Scheduler/ParkrunMap.Scheduler.csproj", "src/ParkrunMap.Scheduler/"]
COPY ["src/ParkrunMap.ServiceDefaults/ParkrunMap.ServiceDefaults.csproj", "src/ParkrunMap.ServiceDefaults/"]
COPY ["src/ParkrunMap.Contracts/ParkrunMap.Contracts.csproj", "src/ParkrunMap.Contracts/"]
COPY ["src/ParkrunMap.Data.Mongo/ParkrunMap.Data.Mongo.csproj", "src/ParkrunMap.Data.Mongo/"]
COPY ["src/ParkrunMap.Scraping/ParkrunMap.Scraping.csproj", "src/ParkrunMap.Scraping/"]
COPY ["src/ParkrunMap.Domain/ParkrunMap.Domain.csproj", "src/ParkrunMap.Domain/"]

RUN dotnet restore "src/ParkrunMap.Scheduler/ParkrunMap.Scheduler.csproj"

COPY src/ src/

WORKDIR "/src/src/ParkrunMap.Scheduler"
RUN dotnet publish "ParkrunMap.Scheduler.csproj" -c Release -o /app/publish --no-restore

# Runtime stage — lightweight aspnet image (no Playwright needed; scheduling only)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ParkrunMap.Scheduler.dll"]

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/ParkrunMap.Api/ParkrunMap.Api.csproj", "src/ParkrunMap.Api/"]
COPY ["src/ParkrunMap.ServiceDefaults/ParkrunMap.ServiceDefaults.csproj", "src/ParkrunMap.ServiceDefaults/"]
COPY ["src/ParkrunMap.Domain/ParkrunMap.Domain.csproj", "src/ParkrunMap.Domain/"]
COPY ["src/ParkrunMap.Contracts/ParkrunMap.Contracts.csproj", "src/ParkrunMap.Contracts/"]
COPY ["src/ParkrunMap.Data.Mongo/ParkrunMap.Data.Mongo.csproj", "src/ParkrunMap.Data.Mongo/"]

RUN dotnet restore "src/ParkrunMap.Api/ParkrunMap.Api.csproj"

COPY src/ src/

WORKDIR "/src/src/ParkrunMap.Api"
RUN dotnet publish "ParkrunMap.Api.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN adduser --disabled-password --no-create-home appuser
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "ParkrunMap.Api.dll"]

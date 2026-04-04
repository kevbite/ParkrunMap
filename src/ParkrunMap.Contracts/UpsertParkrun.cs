namespace ParkrunMap.Contracts;

public record UpsertParkrun(
    int GeoXmlId,
    string Name,
    string Domain,
    string Path,
    string Region,
    string Country,
    double Latitude,
    double Longitude);

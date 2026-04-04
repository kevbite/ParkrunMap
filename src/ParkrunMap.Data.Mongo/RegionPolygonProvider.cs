using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo;

public static class RegionPolygonProvider
{
    // Each polygon is an array of [longitude, latitude] pairs forming a closed ring
    public static double[][] GetPolygon(ParkrunRegion region) => region switch
    {
        ParkrunRegion.UK => UkPolygon,
        ParkrunRegion.Scotland => ScotlandPolygon,
        ParkrunRegion.Wales => WalesPolygon,
        ParkrunRegion.NorthernIreland => NorthernIrelandPolygon,
        ParkrunRegion.NorthEastEngland => NorthEastEnglandPolygon,
        ParkrunRegion.NorthWestEngland => NorthWestEnglandPolygon,
        ParkrunRegion.YorkshireAndHumber => YorkshirePolygon,
        ParkrunRegion.EastMidlands => EastMidlandsPolygon,
        ParkrunRegion.WestMidlands => WestMidlandsPolygon,
        ParkrunRegion.EastEngland => EastEnglandPolygon,
        ParkrunRegion.London => LondonPolygon,
        ParkrunRegion.SouthEastEngland => SouthEastEnglandPolygon,
        ParkrunRegion.SouthWestEngland => SouthWestEnglandPolygon,
        _ => throw new ArgumentOutOfRangeException(nameof(region), region, null)
    };

    // Approximate bounding polygon for the whole UK
    private static readonly double[][] UkPolygon =
    [
        [-8.0, 49.0], [2.0, 49.0], [2.0, 61.0], [-8.0, 61.0], [-8.0, 49.0]
    ];

    private static readonly double[][] ScotlandPolygon =
    [
        [-7.7, 54.6], [-1.8, 54.6], [0.0, 57.7], [-6.3, 58.7], [-7.7, 57.0], [-7.7, 54.6]
    ];

    private static readonly double[][] WalesPolygon =
    [
        [-5.4, 51.3], [-2.6, 51.3], [-2.6, 53.5], [-5.4, 53.5], [-5.4, 51.3]
    ];

    private static readonly double[][] NorthernIrelandPolygon =
    [
        [-8.2, 53.9], [-5.4, 53.9], [-5.4, 55.4], [-8.2, 55.4], [-8.2, 53.9]
    ];

    private static readonly double[][] NorthEastEnglandPolygon =
    [
        [-2.5, 54.4], [0.0, 54.4], [0.0, 55.9], [-2.5, 55.9], [-2.5, 54.4]
    ];

    private static readonly double[][] NorthWestEnglandPolygon =
    [
        [-3.7, 53.3], [-1.7, 53.3], [-1.7, 55.0], [-3.7, 55.0], [-3.7, 53.3]
    ];

    private static readonly double[][] YorkshirePolygon =
    [
        [-2.6, 53.3], [0.2, 53.3], [0.2, 54.6], [-2.6, 54.6], [-2.6, 53.3]
    ];

    private static readonly double[][] EastMidlandsPolygon =
    [
        [-2.0, 52.0], [0.8, 52.0], [0.8, 53.5], [-2.0, 53.5], [-2.0, 52.0]
    ];

    private static readonly double[][] WestMidlandsPolygon =
    [
        [-3.3, 51.9], [-1.0, 51.9], [-1.0, 53.2], [-3.3, 53.2], [-3.3, 51.9]
    ];

    private static readonly double[][] EastEnglandPolygon =
    [
        [-0.5, 51.5], [1.8, 51.5], [1.8, 53.4], [-0.5, 53.4], [-0.5, 51.5]
    ];

    private static readonly double[][] LondonPolygon =
    [
        [-0.55, 51.28], [0.35, 51.28], [0.35, 51.72], [-0.55, 51.72], [-0.55, 51.28]
    ];

    private static readonly double[][] SouthEastEnglandPolygon =
    [
        [-1.8, 50.7], [1.8, 50.7], [1.8, 51.8], [-1.8, 51.8], [-1.8, 50.7]
    ];

    private static readonly double[][] SouthWestEnglandPolygon =
    [
        [-6.0, 49.8], [-1.5, 49.8], [-1.5, 52.0], [-6.0, 52.0], [-6.0, 49.8]
    ];
}

namespace ParkrunMap.FunctionsApp.QueryParkrunsByBox
{
    public interface IPolygonCreator
    {
        double[,] FromBox(double lat1, double lon1, double lat2, double lon2);
    }
}
using Newtonsoft.Json;

namespace ParkrunMap.FunctionsApp.QueryParkrunsByBox;

public class ParkrunSpecialEventsResponseObject
{
    [JsonProperty(PropertyName = "christmasDay")]
    public int[] ChristmasDay { get; set; }

    [JsonProperty(PropertyName = "newYearsDay")]
    public int[] NewYearsDay { get; set; }
}
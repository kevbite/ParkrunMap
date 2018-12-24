using System.Xml.Linq;

namespace ParkrunMap.Scraping.Parkruns
{
    public class ParkrunXElementValidator
    {
        public bool IsValid(XElement xElement)
        {
            if (string.IsNullOrEmpty((string)xElement.Attribute("m")))
            {
                return false;
            }
            if (string.IsNullOrEmpty((string)xElement.Attribute("n")))
            {
                return false;
            }
            if (string.IsNullOrEmpty((string)xElement.Attribute("la")))
            {
                return false;
            }
            if (string.IsNullOrEmpty((string)xElement.Attribute("lo")))
            {
                return false;
            }
            if (string.IsNullOrEmpty((string)xElement.Attribute("r")))
            {
                return false;
            }

            return true;
        }
    }
}
using System.Xml.Linq;

namespace tcxboi.TCX;

public class Position
{
    protected internal static Position Parse(XElement element, XNamespace ns)
    {
        var latitude = float.Parse(element.Element(ns + "LatitudeDegrees")!.Value);
        var longitude = float.Parse(element.Element(ns + "LongitudeDegrees")!.Value);

        return new Position(latitude, longitude);
    }

    public Position(float latitude, float longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
    public float Longitude { get; init; }

    public float Latitude { get; init; }

}
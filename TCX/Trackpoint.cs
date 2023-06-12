using System.Xml.Linq;

namespace tcxboi.TCX;

public class Trackpoint
{
    private static XNamespace activityNs = XNamespace.Get("http://www.garmin.com/xmlschemas/ActivityExtension/v2");
    
    protected internal static Trackpoint Parse(XElement element, XNamespace ns)
    {
        var time = element.Descendants(ns + "Time").First().Value;
        var altitudeMeters = element.Element(ns + "AltitudeMeters")!.Value;
        var distanceMeters = element.Element(ns + "DistanceMeters")!.Value;

        var bpm = element
            .Element(ns + "HeartRateBpm")!
            .Element(ns + "Value")!
            .Value;

        var runCadence = element
            .Element(ns + "Extensions")!
            .Element(activityNs + "TPX")!
            .Element(activityNs + "RunCadence")!
            .Value;

        return new Trackpoint
        {
            Time = DateTime.Parse(time),
            Position = Position.Parse(element.Descendants(ns + "Position").First(), ns),
            AltitudeMeters = float.Parse(altitudeMeters),
            DistanceMeters = float.Parse(distanceMeters),
            BPM = byte.Parse(bpm),
            RunCadence = byte.Parse(runCadence),
        };
    }

    public byte RunCadence { get; private init; }

    public byte BPM { get; private init; }

    public float DistanceMeters { get; private init; }

    public float AltitudeMeters { get; private init; }

    public DateTime Time { get; private init; }
    public Position Position { get; private init; }

    public Trackpoint InterpolateBetween(Trackpoint other, DateTime timestamp)
    {
        var gap = other.Time - Time;
        var start = timestamp - Time;
        var by = start.PercentageOf(gap);
        
        var latitude = by.Lerp(Position.Latitude, other.Position.Latitude);
        var longitude = by.Lerp(Position.Longitude, other.Position.Longitude);
        var altitudeMeters = by.Lerp(AltitudeMeters, other.AltitudeMeters);
        var distanceMeters = by.Lerp(DistanceMeters, other.DistanceMeters);
        var bpm = by.Lerp(BPM, other.BPM);
        var runCadence = by.Lerp(RunCadence, other.RunCadence);

        return new Trackpoint
        {
            Time = timestamp,
            Position = new Position(latitude, longitude),
            AltitudeMeters = altitudeMeters,
            DistanceMeters = distanceMeters,
            BPM = bpm,
            RunCadence = runCadence,
        };
    }
}
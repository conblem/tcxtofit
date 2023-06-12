using System.Xml.Linq;

namespace tcxboi.TCX;

public class Activity
{
    public string Sport { get; private init; }
    public string Id { get; private init; }
    public IEnumerable<Trackpoint> Trackpoints { get; private init; }
    
    public Activity WithTrackpoints(IEnumerable<Trackpoint> trackpoints)
    {
        return new Activity
        {
            Sport = Sport,
            Id = Id,
            StartTime = StartTime,
            StopTime = StopTime,
            DistanceMeters = DistanceMeters,
            Trackpoints = trackpoints,
        };
    }

    protected internal static Activity Parse(XElement element, XNamespace ns)
    {
        var sport = element.Attribute("Sport")!.Value;
        var id = element.Element(ns + "Id")!.Value;

        var laps = element.Elements(ns + "Lap").ToArray();
        if (laps.Length > 1)
        {
            throw new NotImplementedException("Multi-lap activities are not supported");
        }

        var lap = laps.First();

        var startTime = DateTime.Parse(lap.Attribute("StartTime")!.Value);

        var totalTimeSeconds = lap.Element(ns + "TotalTimeSeconds")!.Value;

        var stopTime = startTime.AddSeconds(double.Parse(totalTimeSeconds));

        var tracks = lap.Elements(ns + "Track").ToArray();
        if (tracks.Length > 1)
        {
            throw new NotImplementedException("Multiple tracks are not supported");
        }

        var trackpoints = tracks
            .First()
            .Elements(ns + "Trackpoint")
            .Select(trackpoint => Trackpoint.Parse(trackpoint, ns))
            .ToArray();

        var distanceMeters = lap.Element(ns + "DistanceMeters")?.Value;

        return new Activity
        {
            Sport = sport,
            Id = id,
            StartTime = startTime,
            StopTime = stopTime,
            DistanceMeters = distanceMeters != null ? float.Parse(distanceMeters) : trackpoints.Last().DistanceMeters,
            Trackpoints = trackpoints,
        };
    }

    public float TotalTimeSeconds => (float) (StopTime - StartTime).TotalSeconds;
    public DateTime StopTime { get; private init; }

    public float DistanceMeters { get; private init; }

    public DateTime StartTime { get; private init; }
}
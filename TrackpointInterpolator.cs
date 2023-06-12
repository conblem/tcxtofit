using tcxboi.TCX;

namespace tcxboi;

public class TrackpointInterpolator
{
    private readonly Activity _activity;

    public TrackpointInterpolator(Activity activity)
    {
        _activity = activity;
    }

    public IEnumerable<Trackpoint> Interpolate()
    {
        var current = _activity.Trackpoints.First().Time;
        var end = _activity.Trackpoints.Last().Time;

        using var trackpoints = _activity.Trackpoints
            .WithNextSkipLast()
            .GetEnumerator();
        
        // add error handling
        trackpoints.MoveNext();
        
        var interpolatedTrackpoints = new List<Trackpoint>();
        while (current <= end)
        {
            if (current >= trackpoints.Current.Item2.Time && !trackpoints.MoveNext())
            {
                break;
            }

            var (currentTrackpoint, nextTrackpoint) = trackpoints.Current;

            var interpolatedTrackpoint = currentTrackpoint.InterpolateBetween(nextTrackpoint, current);
            interpolatedTrackpoints.Add(interpolatedTrackpoint);
            
            current = current.AddSeconds(1);
        }

        return interpolatedTrackpoints;
    }
}
using tcxboi.TCX;

namespace tcxboi;

public class ZoneCalculator
{
    private readonly Activity _activity;

    public ZoneCalculator(Activity activity)
    {
        _activity = activity;
    }

    public int[] Calculate()
    {
        var result = new int[5];
        
        var trackpoints = _activity.Trackpoints.ToArray();

        var trackpointsWithLength = trackpoints.WithNextSkipLast().Select(tuple =>
        {
            var duration = tuple.Item2.Time - tuple.Item1.Time;
            return (tuple.Item1, duration.TotalSeconds);
        });
        
        foreach (var (trackpoint, length) in trackpointsWithLength)
        {
            var index = trackpoint.BPM switch
            {
                < 143 => 0,
                < 157 => 1,
                < 170 => 2,
                < 184 => 3,
                _ => 4
            };
            result[index] += (int) Math.Round(length);
        }

        return result;
    }
    
}

using DynastreamDateTime = Dynastream.Fit.DateTime;

namespace tcxboi;

public static class Extensions
{
    public static float Lerp(this float by, float firstFloat, float secondFloat)
    {
        return firstFloat * (1 - by) + secondFloat * by;
    }
    
    public static byte Lerp(this float by, byte firstByte, byte secondByte)
    {
        var result = by.Lerp((float) firstByte, (float) secondByte);
        return (byte) Math.Round(result);
    }

    public static float PercentageOf(this TimeSpan smallerValue, TimeSpan biggerValue)
    {
        if (smallerValue == biggerValue)
        {
            return 1;
        }
        return (float) (smallerValue / biggerValue);
    }
    
    public static DynastreamDateTime ToDynastream(this DateTime dateTime)
    {
        return new DynastreamDateTime(dateTime.ToUniversalTime());
    }

    public static IEnumerable<(TSource, TSource)> WithNextSkipLast<TSource>(this IEnumerable<TSource> enumerable)
    {
        using var enumerator = enumerable.GetEnumerator();

        if (!enumerator.MoveNext())
        {
            yield break;
        }

        var current = enumerator.Current;
        while (enumerator.MoveNext())
        {
            yield return (current, enumerator.Current);
            current = enumerator.Current;
        }
    }
}
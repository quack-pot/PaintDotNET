namespace PaintDotNET.Core.Math;

public class MathGen
{
    public static T Min<T>(T a, T b) where T : IComparable<T> => a.CompareTo(b) < 0 ? a : b;
    public static T Max<T>(T a, T b) where T : IComparable<T> => a.CompareTo(b) > 0 ? a : b;
    public static T Clamp<T>(T value, T min, T max) where T : IComparable<T> => Max(min, Min(max, value));
}

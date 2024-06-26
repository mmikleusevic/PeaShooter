using Unity.Mathematics;

public static class MathExtensions
{
    private const float EPSILON = math.EPSILON;

    public static bool Approximately(float a, float b)
    {
        return math.abs(a - b) < EPSILON;
    }

    public static bool Approximately(double a, double b)
    {
        return math.abs(a - b) < EPSILON;
    }

    public static bool Approximately(float3 a, float3 b)
    {
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }
}

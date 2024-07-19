using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public static class MathExtensions
{
    private const float EPSILON = 1e-6f;

    [BurstCompile]
    public static bool Approximately(in float a, in float b)
    {
        return math.abs(a - b) < EPSILON;
    }

    [BurstCompile]
    public static bool Approximately(in double a, in double b)
    {
        return math.abs(a - b) < EPSILON;
    }

    [BurstCompile]
    public static bool Approximately(in float3 a, in float3 b)
    {
        return math.all(math.abs(a - b) < EPSILON);
    }
}

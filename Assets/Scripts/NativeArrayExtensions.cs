using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public static class NativeArrayExtensions
{
    [BurstCompile]
    public static bool IsEmpty<T>(this NativeArray<T> array) where T : struct
    {
        return array.Length == 0;
    }
}
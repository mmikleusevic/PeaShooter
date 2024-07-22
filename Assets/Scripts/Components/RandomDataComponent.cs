using Unity.Entities;
using Unity.Mathematics;

public struct RandomDataComponent : IComponentData
{
    public Random seed;

    public float3 minimumPosition;
    public float3 maximumPosition;
    public float3 nextPosition => seed.NextFloat3(minimumPosition, maximumPosition);
}
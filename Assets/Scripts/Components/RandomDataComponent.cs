using Unity.Entities;
using Unity.Mathematics;

public struct RandomDataComponent : IComponentData
{
    public Random value;

    public float3 minimumPosition;
    public float3 maximumPosition;
    public float3 nextPosition => value.NextFloat3(minimumPosition, maximumPosition);
}
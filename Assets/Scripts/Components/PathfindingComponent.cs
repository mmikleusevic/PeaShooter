using Unity.Entities;
using Unity.Mathematics;

public struct PathfindingComponent : IComponentData
{
    public float3 startPosition;
    public float3 endPosition;
}
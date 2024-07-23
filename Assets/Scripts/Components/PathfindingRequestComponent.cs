using Unity.Entities;
using Unity.Mathematics;

public struct PathfindingRequestComponent : IComponentData
{
    public float2 start;
    public float2 end;
    public Entity requester;
}
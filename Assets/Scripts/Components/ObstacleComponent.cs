using Unity.Entities;
using Unity.Mathematics;

public enum ObstacleShape
{
    Circle,
    Rectangle
}

public struct ObstacleComponent : IComponentData
{
    public float2 position;
    public ObstacleShape shape;
    public float2 size;
}

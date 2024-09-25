using Unity.Entities;
using Unity.Mathematics;

public struct PlayerComponent : IComponentData
{
    public float moveSpeed;
    public float rotationSpeed;
    public int2 position;
}
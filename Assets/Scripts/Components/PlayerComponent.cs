using Unity.Entities;
using Unity.Mathematics;

public struct PlayerComponent : IComponentData
{
    public float moveSpeed;
    public float rotationSpeed;
    public float3 moveDirection;
}

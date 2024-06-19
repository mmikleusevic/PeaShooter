using Unity.Entities;
using Unity.Mathematics;

public struct PlayerComponent : IComponentData
{
    public float moveSpeed;
    public float3 moveDirection;
}

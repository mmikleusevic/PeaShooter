using Unity.Entities;
using Unity.Mathematics;

public struct EnemyComponent : IComponentData
{
    public float3 moveDirection;
    public float moveSpeed;
}

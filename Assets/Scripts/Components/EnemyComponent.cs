using Unity.Entities;
using Unity.Mathematics;

public struct EnemyComponent : IComponentData
{
    public float moveSpeed;
    public float3 moveDirection;
}

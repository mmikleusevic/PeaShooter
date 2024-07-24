using Unity.Entities;
using Unity.Mathematics;

public struct EnemyComponent : IComponentData
{
    public float moveSpeed;
    public float2 position;
    public float2 targetPosition;
    public int currentPathIndex;
}
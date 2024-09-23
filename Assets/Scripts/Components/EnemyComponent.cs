using Unity.Entities;
using Unity.Mathematics;

public struct EnemyComponent : IComponentData
{
    public int2 gridPosition;
    public float3 position;
    public float moveSpeed;
    public int currentPathIndex;
    public float moveTimer;
    public float moveTimerTarget;
    public byte isFullySpawned;
}
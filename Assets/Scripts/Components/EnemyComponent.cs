using Unity.Entities;
using Unity.Mathematics;

public struct EnemyComponent : IComponentData
{
    public float moveSpeed;
    public int2 position;
    public int currentPathIndex;
}
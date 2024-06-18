using Unity.Entities;
using Unity.Mathematics;

public struct EnemySpawnerComponent : IComponentData
{
    public Entity prefab;
    public float3 spawnPosition;
    public float nextSpawnTime;
    public float spawnRate;
}

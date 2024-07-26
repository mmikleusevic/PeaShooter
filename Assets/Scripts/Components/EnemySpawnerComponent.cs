using Unity.Entities;

public struct EnemySpawnerComponent : IComponentData
{
    public Entity prefab;
    public float nextSpawnTime;
    public float spawnRate;
    public float speed;
    public float scale;
}
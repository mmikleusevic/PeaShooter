using Unity.Entities;

public struct EnemySpawnerComponent : IComponentData
{
    public Entity prefab;
    public float nextSpawnTime;
    public float spawnRate;
    public float moveSpeed;
    public float scale;
    public float enemyMoveTimerTarget;
    public double startTime;
    public float destroySpawnerTimerTarget;
}
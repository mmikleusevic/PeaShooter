using Unity.Entities;

public struct ObstacleSpawnerComponent : IComponentData
{
    public Entity prefab;
    public int numberToSpawn;
    public float scale;
}
using Unity.Entities;

public struct ObstacleSpawnerComponent : IComponentData
{
    public Entity prefabEntity;
    public int numberToSpawn;
}
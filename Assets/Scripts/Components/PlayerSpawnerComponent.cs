using Unity.Entities;
using Unity.Mathematics;

public struct PlayerSpawnerComponent : IComponentData
{
    public Entity prefab;
    public float3 spawnPosition;
}

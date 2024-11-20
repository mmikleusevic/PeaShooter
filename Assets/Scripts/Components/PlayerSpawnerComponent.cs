using Unity.Entities;
using Unity.Mathematics;

public struct PlayerSpawnerComponent : IComponentData
{
    public Entity prefabEntity;
    public Entity startingAbilityPrefabEntity;
    public float3 position;
    public quaternion rotation;
    public float scale;
}
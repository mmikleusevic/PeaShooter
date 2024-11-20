using Unity.Entities;
using Unity.Mathematics;

public struct GridSpawnerComponent : IComponentData
{
    public Entity prefabEntity;
    public int2 size;
}
using Unity.Entities;
using Unity.Mathematics;

public struct GridSpawnerComponent : IComponentData
{
    public Entity prefab;
    public int2 size;
}
using Unity.Entities;
using Unity.Mathematics;

public struct PlaneSpawnerComponent : IComponentData
{
    public Entity prefab;
    public float3 position;
    public quaternion rotation;
    public float planeSize;
}

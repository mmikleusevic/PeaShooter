#region

using Unity.Entities;
using Unity.Mathematics;

#endregion

namespace Components
{
    public struct PlaneSpawnerComponent : IComponentData
    {
        public Entity prefab;
        public float3 position;
        public quaternion rotation;
    }
}
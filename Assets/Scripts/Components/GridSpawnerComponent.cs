using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct GridSpawnerComponent : IComponentData
    {
        public Entity prefabEntity;
        public int2 size;
    }
}
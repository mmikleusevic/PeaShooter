#region

using Unity.Entities;
using Unity.Mathematics;

#endregion

namespace Components
{
    public struct GridSpawnerComponent : IComponentData
    {
        public Entity prefabEntity;
        public int2 size;
    }
}
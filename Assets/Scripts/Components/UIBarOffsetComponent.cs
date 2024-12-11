using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct UIBarOffsetComponent : IComponentData
    {
        public float3 offset;
    }
}
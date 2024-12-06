using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct UIBarOffset : IComponentData
    {
        public float3 offset;
    }
}
#region

using Unity.Entities;
using Unity.Mathematics;

#endregion

namespace Components
{
    public struct UIBarOffset : IComponentData
    {
        public float3 offset;
    }
}
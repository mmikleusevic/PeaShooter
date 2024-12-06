using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct InputComponent : IComponentData
    {
        public float2 moveInput;
    }
}
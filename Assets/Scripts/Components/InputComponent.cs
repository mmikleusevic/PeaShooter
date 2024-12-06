#region

using Unity.Entities;
using Unity.Mathematics;

#endregion

namespace Components
{
    public struct InputComponent : IComponentData
    {
        public float2 moveInput;
    }
}
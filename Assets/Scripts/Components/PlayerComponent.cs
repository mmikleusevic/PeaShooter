#region

using Unity.Entities;
using Unity.Mathematics;

#endregion

namespace Components
{
    public struct PlayerComponent : IComponentData
    {
        public float moveSpeed;
        public float rotationSpeed;
        public int2 gridPosition;
        public float3 position;
    }
}
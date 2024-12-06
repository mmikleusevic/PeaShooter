using Unity.Entities;
using Unity.Mathematics;

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
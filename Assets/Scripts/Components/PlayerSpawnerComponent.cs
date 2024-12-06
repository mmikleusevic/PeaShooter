using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct PlayerSpawnerComponent : IComponentData
    {
        public Entity prefabEntity;
        public Entity startingAbilityPrefabEntity;
        public float3 position;
        public quaternion rotation;
        public float scale;
    }
}
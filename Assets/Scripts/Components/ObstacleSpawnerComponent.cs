using Unity.Entities;

namespace Components
{
    public struct ObstacleSpawnerComponent : IComponentData
    {
        public Entity prefabEntity;
        public int numberToSpawn;
    }
}
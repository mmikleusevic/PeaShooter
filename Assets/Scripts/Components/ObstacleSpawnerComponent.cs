using Unity.Entities;

namespace Components
{
    public struct ObstacleSpawnerComponent : IComponentData
    {
        public Entity prefab1Entity;
        public Entity prefab2Entity;
        public int numberToSpawn;
    }
}
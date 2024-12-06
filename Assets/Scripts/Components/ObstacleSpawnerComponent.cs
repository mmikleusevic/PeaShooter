#region

using Unity.Entities;

#endregion

namespace Components
{
    public struct ObstacleSpawnerComponent : IComponentData
    {
        public Entity prefabEntity;
        public int numberToSpawn;
    }
}
#region

using Unity.Entities;

#endregion

namespace Components
{
    public struct EnemySpawnerComponent : IComponentData
    {
        public Entity prefabEntity;
        public float nextSpawnTime;
        public float spawnRate;
        public float moveSpeed;
        public float scale;
        public float enemyMoveTimerTarget;
        public double startTime;
        public float destroySpawnerTimerTarget;
    }
}
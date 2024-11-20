using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private float spawnRate;
        [SerializeField] [Range(100, 500)] private float speed;
        [SerializeField] private float enemyMoveTimerTarget;
        [SerializeField] private float destroySpawnerTimerTarget;

        public class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
        {
            public override void Bake(EnemySpawnerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.None);

                DependsOn(authoring.prefab);

                AddComponent(entity, new EnemySpawnerComponent
                {
                    prefabEntity = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                    nextSpawnTime = authoring.spawnRate,
                    spawnRate = authoring.spawnRate,
                    moveSpeed = authoring.speed,
                    scale = authoring.prefab.transform.localScale.x,
                    enemyMoveTimerTarget = authoring.enemyMoveTimerTarget,
                    destroySpawnerTimerTarget = authoring.destroySpawnerTimerTarget
                });
            }
        }
    }
}
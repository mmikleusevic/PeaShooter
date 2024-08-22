using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float spawnRate;
    [SerializeField] private float speed;
    [SerializeField] private float enemyMoveTimerTarget;

    public class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new EnemySpawnerComponent
            {
                prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                nextSpawnTime = authoring.spawnRate,
                spawnRate = authoring.spawnRate,
                moveSpeed = authoring.speed,
                scale = authoring.prefab.transform.localScale.x,
                enemyMoveTimerTarget = authoring.enemyMoveTimerTarget
            });
        }
    }
}
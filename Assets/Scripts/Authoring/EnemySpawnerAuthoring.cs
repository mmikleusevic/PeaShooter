using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public float spawnRate;
}

public class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
{
    public override void Bake(EnemySpawnerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new EnemySpawnerComponent
        {
            prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
            nextSpawnTime = 0.0f,
            spawnRate = authoring.spawnRate
        });
    }
}

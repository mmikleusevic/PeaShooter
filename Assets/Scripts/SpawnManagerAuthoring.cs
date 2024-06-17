using Unity.Entities;
using UnityEngine;

public class SpawnManagerAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public float spawnRate; 
}

public class SpawnManagerBaker : Baker<SpawnManagerAuthoring>
{
    public override void Bake(SpawnManagerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new SpawnManager
        {
            prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
            spawnPosition = authoring.transform.position,
            nextSpawnTime = 0.0f,
            spawnRate = authoring.spawnRate
        });
    }
}

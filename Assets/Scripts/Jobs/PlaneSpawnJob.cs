using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct PlaneSpawnJob : IJobEntity
{
    public EntityCommandBuffer ecb;

    private void Execute(in PlaneSpawnerComponent planeSpawner, in Entity spawnerEntity)
    {
        Entity spawnedEntity = ecb.Instantiate(planeSpawner.prefab);

        ecb.SetComponent(spawnedEntity, new LocalTransform
        {
            Position = planeSpawner.position,
            Rotation = planeSpawner.rotation,
            Scale = 1f
        });

        ecb.AddComponent(spawnedEntity, new PlaneComponent());

        ecb.DestroyEntity(spawnerEntity);
    }
}
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct PlayerSpawnJob : IJobEntity
{
    public EntityCommandBuffer ecb;

    private void Execute(in PlayerSpawnerComponent playerSpawner, Entity spawnerEntity)
    {
        Entity spawnedEntity = ecb.Instantiate(playerSpawner.prefab);

        ecb.SetComponent(spawnedEntity, new LocalTransform
        {
            Position = playerSpawner.position,
            Rotation = playerSpawner.rotation,
            Scale = playerSpawner.scale
        });

        ecb.AddComponent(spawnedEntity, new PlayerAliveComponent());

        ecb.DestroyEntity(spawnerEntity);
    }
}
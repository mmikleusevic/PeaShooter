using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(PlaneSpawnerSystem))]
public partial struct PlayerSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        SpawnEntity(ref state);
    }

    [BurstCompile]
    private void SpawnEntity(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerSpawnerComponent>(out Entity entity)) return;

        state.Enabled = false;

        RefRW<PlayerSpawnerComponent> spawner = SystemAPI.GetComponentRW<PlayerSpawnerComponent>(entity);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        Entity spawnedEntity = ecb.Instantiate(spawner.ValueRO.prefab);

        ecb.SetComponent(spawnedEntity, new LocalTransform
        {
            Position = spawner.ValueRO.spawnPosition,
            Rotation = quaternion.identity,
            Scale = 1f,
        });

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
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

        Entity spawnedEntity = state.EntityManager.Instantiate(spawner.ValueRO.prefab);

        state.EntityManager.SetComponentData(spawnedEntity, new LocalTransform
        {
            Position = spawner.ValueRO.spawnPosition,
            Rotation = quaternion.identity,
            Scale = 1f,
        });
    }
}

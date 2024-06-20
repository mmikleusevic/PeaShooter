using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct PlayerSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<PlayerSpawnerComponent>(out Entity entity)) return;

        state.Enabled = false;

        RefRW<PlayerSpawnerComponent> spawner = SystemAPI.GetComponentRW<PlayerSpawnerComponent>(entity);

        Entity spawnedEntity = state.EntityManager.Instantiate(spawner.ValueRO.prefab);

        state.EntityManager.SetComponentData(spawnedEntity, new LocalTransform
        {
            Position = spawner.ValueRO.position,
            Rotation = spawner.ValueRO.rotation,
            Scale = 1f,
        });
    }
}

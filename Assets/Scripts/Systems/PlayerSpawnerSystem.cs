using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct PlayerSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerSpawnerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        Entity entity = SystemAPI.GetSingletonEntity<PlayerSpawnerComponent>();

        RefRO<PlayerSpawnerComponent> spawner = SystemAPI.GetComponentRO<PlayerSpawnerComponent>(entity);

        Entity spawnedEntity = state.EntityManager.Instantiate(spawner.ValueRO.prefab);

        state.EntityManager.SetComponentData(spawnedEntity, new LocalTransform
        {
            Position = spawner.ValueRO.position,
            Rotation = spawner.ValueRO.rotation,
            Scale = 1f,
        });
    }
}

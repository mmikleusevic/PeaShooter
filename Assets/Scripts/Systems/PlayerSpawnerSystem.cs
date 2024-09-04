using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct PlayerSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerSpawnerComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        Entity playerSpawnerEntity = SystemAPI.GetSingletonEntity<PlayerSpawnerComponent>();

        RefRO<PlayerSpawnerComponent> spawner = SystemAPI.GetComponentRO<PlayerSpawnerComponent>(playerSpawnerEntity);

        Entity spawnedEntity = state.EntityManager.Instantiate(spawner.ValueRO.prefab);

        state.EntityManager.SetComponentData(spawnedEntity, new LocalTransform
        {
            Position = spawner.ValueRO.position,
            Rotation = spawner.ValueRO.rotation,
            Scale = spawner.ValueRO.scale
        });

        state.EntityManager.DestroyEntity(playerSpawnerEntity);
    }
}
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct PlaneSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlaneSpawnerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity planeSpawnerEntity = SystemAPI.GetSingletonEntity<PlaneSpawnerComponent>();

        RefRO<PlaneSpawnerComponent> spawner = SystemAPI.GetComponentRO<PlaneSpawnerComponent>(planeSpawnerEntity);

        Entity spawnedEntity = state.EntityManager.Instantiate(spawner.ValueRO.prefab);

        state.EntityManager.SetComponentData(spawnedEntity, new LocalTransform
        {
            Position = spawner.ValueRO.position,
            Rotation = spawner.ValueRO.rotation,
            Scale = 1f,
        });

        state.EntityManager.DestroyEntity(planeSpawnerEntity);
    }
}
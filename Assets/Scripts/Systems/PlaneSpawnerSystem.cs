using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct PlaneSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        SpawnPlane(ref state);
    }

    private void SpawnPlane(ref SystemState state)
    {
        state.Enabled = false;

        if (!SystemAPI.TryGetSingletonEntity<PlaneSpawnerComponent>(out Entity entity)) return;

        RefRO<PlaneSpawnerComponent> spawner = SystemAPI.GetComponentRO<PlaneSpawnerComponent>(entity);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        Entity spawnedEntity = ecb.Instantiate(spawner.ValueRO.prefab);

        ecb.SetComponent(spawnedEntity, new LocalTransform
        {
            Position = spawner.ValueRO.position,
            Rotation = spawner.ValueRO.rotation,
            Scale = 1f,
        });

        ecb.AddComponent(spawnedEntity, new PlaneComponent
        {
            planeSize = spawner.ValueRO.planeSize,
        });

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

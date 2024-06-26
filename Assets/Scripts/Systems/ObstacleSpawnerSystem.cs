using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[BurstCompile]
[UpdateAfter(typeof(PlaneSpawnerSystem))]
public partial struct ObstacleSpawnerSystem : ISystem
{
    private Random random;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = new Random((uint)UnityEngine.Random.Range(1, int.MaxValue));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<ObstacleSpawnerComponent>(out Entity entity)) return;

        state.Enabled = false;

        RefRW<ObstacleSpawnerComponent> spawner = SystemAPI.GetComponentRW<ObstacleSpawnerComponent>(entity);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        for (int i = 0; i < spawner.ValueRO.numberToSpawn; i++)
        {
            Entity spawnedEntity = ecb.Instantiate(spawner.ValueRO.prefab);

            float3 spawnPosition = new float3(random.NextFloat(-Config.Instance.GetPlaneSize(), Config.Instance.GetPlaneSize()), 0,
               random.NextFloat(-Config.Instance.GetPlaneSize(), Config.Instance.GetPlaneSize()));

            ecb.SetComponent(spawnedEntity, new LocalTransform
            {
                Position = spawnPosition,
                Rotation = quaternion.identity,
                Scale = 1f,
            });
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

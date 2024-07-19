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
    private float planeSize;

    public void OnCreate(ref SystemState state)
    {
        random = new Random((uint)UnityEngine.Random.Range(1, uint.MaxValue));

        state.RequireForUpdate<ObstacleSpawnerComponent>();
        state.RequireForUpdate<PlaneConfigComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        PlaneConfigComponent planeConfig = SystemAPI.GetSingleton<PlaneConfigComponent>();
        planeSize = planeConfig.planeSize;

        Entity entity = SystemAPI.GetSingletonEntity<ObstacleSpawnerComponent>();

        RefRO<ObstacleSpawnerComponent> spawner = SystemAPI.GetComponentRO<ObstacleSpawnerComponent>(entity);

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        for (int i = 0; i < spawner.ValueRO.numberToSpawn; i++)
        {
            Entity spawnedEntity = ecb.Instantiate(spawner.ValueRO.prefab);

            float3 spawnPosition = new float3(random.NextFloat(-planeSize, planeSize), 0,
               random.NextFloat(-planeSize, planeSize));

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

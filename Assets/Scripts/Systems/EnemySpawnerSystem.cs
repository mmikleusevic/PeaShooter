using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[BurstCompile]
[UpdateAfter(typeof(PlaneSpawnerSystem))]
public partial struct EnemySpawnerSystem : ISystem
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
        if (!SystemAPI.TryGetSingletonEntity<EnemySpawnerComponent>(out Entity entity)) return;

        RefRW<EnemySpawnerComponent> spawner = SystemAPI.GetComponentRW<EnemySpawnerComponent>(entity);

        if (spawner.ValueRO.nextSpawnTime < SystemAPI.Time.ElapsedTime)
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            Entity spawnedEntity = ecb.Instantiate(spawner.ValueRO.prefab);

            float3 spawnPosition = new float3(random.NextFloat(-Config.Instance.GetPlaneSize() + 1, Config.Instance.GetPlaneSize() - 1), 0,
               random.NextFloat(-Config.Instance.GetPlaneSize() + 1, Config.Instance.GetPlaneSize() - 1));

            ecb.SetComponent(spawnedEntity, new LocalTransform
            {
                Position = spawnPosition,
                Rotation = quaternion.identity,
                Scale = 1f,
            });

            spawner.ValueRW.nextSpawnTime = (float)SystemAPI.Time.ElapsedTime + spawner.ValueRO.spawnRate;

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
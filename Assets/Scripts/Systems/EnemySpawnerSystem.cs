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
    private const float epsilon = 1e-5f;
    private float planeSize;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = new Random((uint)UnityEngine.Random.Range(1, int.MaxValue));
        planeSize = 0f;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        bool planeSet = CheckIfPlaneSet(ref state);

        if (!planeSet) return;

        SpawnEntities(ref state);
    }

    [BurstCompile]
    private bool CheckIfPlaneSet(ref SystemState state)
    {
        bool approximatelyZero = planeSize < epsilon;
        if (approximatelyZero)
        {
            if (!SystemAPI.TryGetSingletonEntity<PlaneComponent>(out Entity planeEntity)) return false;

            RefRO<PlaneComponent> entitySpawner = SystemAPI.GetComponentRO<PlaneComponent>(planeEntity);

            planeSize = entitySpawner.ValueRO.planeSize;

            return true;
        }

        return true;
    }

    [BurstCompile]
    private void SpawnEntities(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<EnemySpawnerComponent>(out Entity entity)) return;

        RefRW<EnemySpawnerComponent> spawner = SystemAPI.GetComponentRW<EnemySpawnerComponent>(entity);

        if (spawner.ValueRO.nextSpawnTime < SystemAPI.Time.ElapsedTime)
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            Entity spawnedEntity = ecb.Instantiate(spawner.ValueRO.prefab);

            float3 spawnPosition = new float3(random.NextFloat(-planeSize, planeSize), random.NextFloat(-planeSize, planeSize), 0f);

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
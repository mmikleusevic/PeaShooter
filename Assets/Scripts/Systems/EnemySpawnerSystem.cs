using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

[BurstCompile]
[UpdateAfter(typeof(PlaneSpawnerSystem))]
public partial struct EnemySpawnerSystem : ISystem
{
    private Random random;
    private float planeSize;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = new Random((uint)UnityEngine.Random.Range(1, int.MaxValue));
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);

        if (MathExtensions.Approximately(planeSize, 0f))
        {
            if (!SystemAPI.TryGetSingleton(out PlaneConfigComponent planeConfig)) return;
            planeSize = planeConfig.planeSize;
        }

        new EnemySpawnJob
        {
            ecb = ecb,
            spawnPosition = new float3
            {
                x = random.NextFloat(-planeSize + 1, planeSize - 1),
                y = 0,
                z = random.NextFloat(-planeSize + 1, planeSize - 1)
            },
            elapsedTime = SystemAPI.Time.ElapsedTime,
        }.ScheduleParallel();
    }

    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb.AsParallelWriter();
    }
}
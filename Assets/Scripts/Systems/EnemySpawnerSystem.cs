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

    public void OnCreate(ref SystemState state)
    {
        random = new Random((uint)UnityEngine.Random.Range(1, uint.MaxValue));

        state.RequireForUpdate<PlaneConfigComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (MathExtensions.Approximately(planeSize, 0f))
        {
            PlaneConfigComponent planeConfigComponent = SystemAPI.GetSingleton<PlaneConfigComponent>();
            planeSize = planeConfigComponent.planeSize;
        }

        new EnemySpawnJob
        {
            ecb = GetEntityCommandBuffer(ref state),
            spawnPosition = new float3
            {
                x = random.NextFloat(-planeSize + 1, planeSize - 1),
                y = 0,
                z = random.NextFloat(-planeSize + 1, planeSize - 1)
            },
            elapsedTime = SystemAPI.Time.ElapsedTime,
        }.ScheduleParallel();
    }

    [BurstCompile]
    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb.AsParallelWriter();
    }
}
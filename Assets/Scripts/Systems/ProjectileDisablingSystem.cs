using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
public partial struct ProjectileDisablingSystem : ISystem
{
    private BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton;
    private EntityQuery projectilesQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        projectilesQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<ProjectileComponent>()
            .Build(ref state);

        state.RequireForUpdate<ProjectileComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        ProjectileDisablingJob job = new ProjectileDisablingJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            ecb = ecb.AsParallelWriter()
        };

        JobHandle handle = job.ScheduleParallel(state.Dependency);
        state.Dependency = handle;
    }
}
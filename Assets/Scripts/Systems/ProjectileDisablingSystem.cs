using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
public partial struct ProjectileDisablingSystem : ISystem
{
    private EntityQuery projectilesQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        projectilesQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<ProjectileComponent>()
            .Build(ref state);

        state.RequireForUpdate<ProjectileComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
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
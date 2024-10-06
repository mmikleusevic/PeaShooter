using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;

[BurstCompile]
[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(ProjectileTargetingSystem))]
public partial struct ProjectileDisablingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ProjectileComponent>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        ProjectileDisablingJob job = new ProjectileDisablingJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            enemyLookup = SystemAPI.GetComponentLookup<EnemyComponent>(true),
            ecb = ecb.AsParallelWriter()
        };

        JobHandle handle = job.ScheduleParallel(state.Dependency);
        state.Dependency = handle;
    }
}
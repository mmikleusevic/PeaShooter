using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
[WithAll(typeof(ProjectileComponent))]
public partial struct ProjectileDisablingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ProjectileComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EndFixedStepSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
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
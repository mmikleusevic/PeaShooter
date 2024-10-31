using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;

[BurstCompile]
[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(CollisionDamageSystem))]
public partial struct ProjectileTargetingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TargetComponent>();
        state.RequireForUpdate<PlayerAliveComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        ProjectileTargetingJob job = new ProjectileTargetingJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            AbilityLookup = SystemAPI.GetComponentLookup<AbilityComponent>(true)
        };

        JobHandle jobHandle = job.ScheduleParallel(state.Dependency);
        state.Dependency = jobHandle;
    }
}
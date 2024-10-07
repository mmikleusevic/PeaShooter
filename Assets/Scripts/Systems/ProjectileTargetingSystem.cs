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
        ProjectileTargetingSystemJob job = new ProjectileTargetingSystemJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };

        JobHandle jobHandle = job.ScheduleParallel(state.Dependency);
        state.Dependency = jobHandle;
    }
}
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(ProjectileTargetingSystem))]
    public partial struct ProjectileDisablingSystem : ISystem
    {
        private ComponentLookup<EnemyComponent> enemyComponentLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ProjectileComponent>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<PlayerAliveComponent>();

            enemyComponentLookup = state.GetComponentLookup<EnemyComponent>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            enemyComponentLookup.Update(ref state);

            EndFixedStepSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            ProjectileDisablingJob job = new ProjectileDisablingJob
            {
                deltaTime = SystemAPI.Time.fixedDeltaTime,
                enemyComponentLookup = enemyComponentLookup,
                ecb = ecb.AsParallelWriter()
            };

            JobHandle handle = job.ScheduleParallel(state.Dependency);
            state.Dependency = handle;
        }
    }
}
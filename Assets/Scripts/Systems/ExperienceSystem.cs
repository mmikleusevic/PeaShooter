using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
public partial struct ExperienceSystem : ISystem
{
    private EntityQuery playerEntityQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        playerEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<PlayerComponent>()
            .Build(state.EntityManager);

        state.RequireForUpdate(playerEntityQuery);
        state.RequireForUpdate<EnemyDeadComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        ExperienceJob job = new ExperienceJob
        {
            playerLookup = SystemAPI.GetComponentLookup<PlayerComponent>(),
            playerEntity = playerEntityQuery.GetSingletonEntity()
        };

        JobHandle jobHandle = job.Schedule(state.Dependency);
        state.Dependency = jobHandle;
    }
}
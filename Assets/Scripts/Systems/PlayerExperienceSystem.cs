using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
public partial struct PlayerExperienceSystem : ISystem
{
    private EntityQuery playerEntityQuery;
    private EntityQuery levelsEntityQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        playerEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<PlayerComponent>()
            .Build(state.EntityManager);

        levelsEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<LevelsComponent>()
            .Build(state.EntityManager);

        state.RequireForUpdate(playerEntityQuery);
        state.RequireForUpdate(levelsEntityQuery);
        state.RequireForUpdate<EnemyDeadComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PlayerExperienceJob job = new PlayerExperienceJob
        {
            experienceLookup = SystemAPI.GetComponentLookup<PlayerExperienceComponent>(),
            playerEntity = playerEntityQuery.GetSingletonEntity(),
            levelsComponent = levelsEntityQuery.GetSingleton<LevelsComponent>()
        };

        JobHandle jobHandle = job.Schedule(state.Dependency);
        state.Dependency = jobHandle;
    }
}
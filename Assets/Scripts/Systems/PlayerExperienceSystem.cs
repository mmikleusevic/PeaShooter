using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
public partial class PlayerExperienceSystem : SystemBase
{
    public event Action<uint, uint> OnGainedExp;
    public event Action OnLevelUp;

    private EntityQuery playerExperienceEntityQuery;
    private EntityQuery levelsEntityQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        playerExperienceEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<PlayerExperienceComponent, PlayerAliveComponent>()
            .Build(EntityManager);

        levelsEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<LevelsComponent>()
            .Build(EntityManager);

        RequireForUpdate(playerExperienceEntityQuery);
        RequireForUpdate(levelsEntityQuery);
        RequireForUpdate<EnemyDeadComponent>();
        RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    protected override void OnUpdate()
    {
        EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);

        LevelsComponent levelsComponent = levelsEntityQuery.GetSingleton<LevelsComponent>();
        RefRW<PlayerExperienceComponent> playerExperience = playerExperienceEntityQuery.GetSingletonRW<PlayerExperienceComponent>();

        foreach (var (experienceComponent, enemyDeadComponent, entity) in SystemAPI.Query<RefRW<EnemyExperienceWorthComponent>, RefRO<EnemyDeadComponent>>()
            .WithEntityAccess())
        {
            uint maxEXP = levelsComponent.levels.Value.experience[levelsComponent.levels.Value.experience.Length - 1];
            uint currentEXP = playerExperience.ValueRO.points + experienceComponent.ValueRO.value;

            ecb.RemoveComponent<EnemyExperienceWorthComponent>(entity);

            if (currentEXP == maxEXP) return;

            uint playerExp = math.min(currentEXP, maxEXP);

            playerExperience.ValueRW.points = playerExp;

            int currentLevel = playerExperience.ValueRO.currentLevel;
            uint currentLevelMaxEXP = levelsComponent.levels.Value.experience[currentLevel - 1];

            if (playerExp >= currentLevelMaxEXP)
            {
                playerExperience.ValueRW.currentLevel++;

                int newCurrentLevel = playerExperience.ValueRW.currentLevel;
                uint newCurrentLevelMaxEXP = levelsComponent.levels.Value.experience[newCurrentLevel - 1];

                OnGainedExp?.Invoke(playerExp, newCurrentLevelMaxEXP);
                OnLevelUp?.Invoke();
            }
            else
            {
                OnGainedExp?.Invoke(playerExp, currentLevelMaxEXP);
            }
        }
    }
}
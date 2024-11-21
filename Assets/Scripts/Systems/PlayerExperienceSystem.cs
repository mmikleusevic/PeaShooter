using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    public partial class PlayerExperienceSystem : SystemBase
    {
        private EntityQuery levelsEntityQuery;

        private EntityQuery playerExperienceEntityQuery;
        public event Action<uint, uint> OnGainedExp;
        public event Action OnLevelUp;

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
            EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(World.Unmanaged);

            LevelsComponent levelsComponent = levelsEntityQuery.GetSingleton<LevelsComponent>();
            RefRW<PlayerExperienceComponent> playerExperienceComponentRW =
                playerExperienceEntityQuery.GetSingletonRW<PlayerExperienceComponent>();

            foreach ((RefRW<EnemyExperienceWorthComponent> experienceComponentRW,
                         Entity enemyEntity) in SystemAPI.Query<RefRW<EnemyExperienceWorthComponent>>()
                         .WithAll<EnemyDeadComponent>()
                         .WithEntityAccess())
            {
                uint maxExp =
                    levelsComponent.levels.Value.experience[levelsComponent.levels.Value.experience.Length - 1];

                uint currentExp = playerExperienceComponentRW.ValueRO.points + experienceComponentRW.ValueRO.experience;

                ecb.RemoveComponent<EnemyExperienceWorthComponent>(enemyEntity);

                if (currentExp == maxExp) return;

                uint playerExp = math.min(currentExp, maxExp);

                playerExperienceComponentRW.ValueRW.points = playerExp;

                int currentLevel = playerExperienceComponentRW.ValueRO.currentLevel;
                uint currentLevelMaxExp = levelsComponent.levels.Value.experience[currentLevel - 1];

                if (playerExp > currentLevelMaxExp)
                {
                    playerExperienceComponentRW.ValueRW.currentLevel++;

                    int newCurrentLevel = playerExperienceComponentRW.ValueRW.currentLevel;
                    uint newCurrentLevelMaxExp = levelsComponent.levels.Value.experience[newCurrentLevel - 1];

                    OnGainedExp?.Invoke(playerExp, newCurrentLevelMaxExp);
                    OnLevelUp?.Invoke();
                }
                else
                {
                    OnGainedExp?.Invoke(playerExp, currentLevelMaxExp);
                }
            }
        }
    }
}
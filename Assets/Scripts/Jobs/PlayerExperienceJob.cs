using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct PlayerExperienceJob : IJobEntity
{
    public ComponentLookup<PlayerExperienceComponent> experienceLookup;

    [ReadOnly] public Entity playerEntity;
    [ReadOnly] public LevelsComponent levelsComponent;

    private void Execute(ref EnemyExperienceWorthComponent experienceComponent, in EnemyDeadComponent enemyDeadComponent)
    {
        if (experienceComponent.isGranted == 1) return;

        experienceComponent.isGranted = 1;

        RefRW<PlayerExperienceComponent> playerComponentRW = experienceLookup.GetRefRW(playerEntity);

        uint maxEXP = levelsComponent.levels.Value.experience[levelsComponent.levels.Value.experience.Length - 1];
        uint currentEXP = playerComponentRW.ValueRW.points + experienceComponent.value;

        if (currentEXP == maxEXP) return;

        uint playerExp = math.min(currentEXP, maxEXP);

        int currentLVL = playerComponentRW.ValueRO.currentLevel;
        uint currentLevelMaxEXP = levelsComponent.levels.Value.experience[currentLVL - 1];

        if (playerExp > currentLevelMaxEXP)
        {
            playerComponentRW.ValueRW.currentLevel++;
            // TODO: Do something
        }

        playerComponentRW.ValueRW.points = playerExp;
    }
}
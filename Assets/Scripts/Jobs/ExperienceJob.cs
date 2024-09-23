using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
public partial struct ExperienceJob : IJobEntity
{
    public ComponentLookup<PlayerComponent> playerLookup;

    [ReadOnly] public Entity playerEntity;

    private void Execute(ref ExperienceComponent experienceComponent, in EnemyDeadComponent enemyDeadComponent)
    {
        if (experienceComponent.isGranted == 1) return;

        experienceComponent.isGranted = 1;

        RefRW<PlayerComponent> playerComponentRW = playerLookup.GetRefRW(playerEntity);
        playerComponentRW.ValueRW.experience += experienceComponent.value;
    }
}
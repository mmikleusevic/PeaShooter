using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct AbilityJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;
    public Entity projectileEntity;

    [ReadOnly] public ComponentLookup<EnemyComponent> enemyLookup;
    [ReadOnly] public NativeArray<Entity> enemyEntities;
    [ReadOnly] public LocalTransform playerTransform;
    [ReadOnly] public float deltaTime;

    private void Execute([ChunkIndexInQuery] int sortKey, ref AbilityComponent ability, in Entity entity)
    {
        if (ability.cooldownRemaining <= 0f)
        {
            EnemyComponent closestEnemy = default;
            Entity closestEnemyEntity = Entity.Null;
            float closestDistance = float.MaxValue;

            foreach (Entity enemyEntity in enemyEntities)
            {
                EnemyComponent enemy = enemyLookup[enemyEntity];

                if (enemy.isFullySpawned == 0) continue;

                float distance = math.distance(playerTransform.Position, enemy.position);

                if (distance < ability.range && distance < closestDistance)
                {
                    closestEnemyEntity = enemyEntity;
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }

            if (closestEnemyEntity == Entity.Null) return;

            if (ability.hasProjectile == 1)
            {
                if (projectileEntity == Entity.Null)
                {
                    projectileEntity = ecb.Instantiate(sortKey, ability.projectileEntity);

                    ecb.AddComponent(sortKey, projectileEntity, new ProjectileAbilityComponent
                    {
                        parentEntity = entity
                    });
                }
                else
                {
                    ecb.SetComponentEnabled<ProjectileComponent>(sortKey, projectileEntity, true);
                }

                ecb.SetComponent(sortKey, projectileEntity, new LocalTransform
                {
                    Position = playerTransform.Position,
                    Rotation = quaternion.identity,
                    Scale = ability.projectileScale
                });

                ecb.AddComponent(sortKey, projectileEntity, new TargetComponent
                {
                    enemy = closestEnemy,
                    enemyEntity = closestEnemyEntity
                });
            }

            //TODO: Need to think what to do with the non projectile based abilities
            ability.cooldownRemaining = ability.cooldown;
        }
        else
        {
            ability.cooldownRemaining -= deltaTime;
        }
    }
}
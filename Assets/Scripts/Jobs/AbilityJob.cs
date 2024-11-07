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

    [ReadOnly] public GridComponent gridComponent;
    [ReadOnly] public ComponentLookup<EnemyComponent> enemyLookup;
    [ReadOnly] public PlayerComponent playerComponent;
    [ReadOnly] public float deltaTime;

    private static readonly int2[] Directions =
    {
        new(-1, 1), new(0, 1), new(1, 1),
        new(-1, 0), new(1, 0),
        new(-1, -1), new(0, -1), new(1, -1)
    };

    private void Execute([ChunkIndexInQuery] int sortKey, ref AbilityComponent ability, in Entity entity)
    {
        if (ability.cooldownRemaining <= 0f)
        {
            if (ability.hasProjectile == 1)
            {
                int2 playerGridPosition = playerComponent.gridPosition;
                Entity closestEnemyEntity = Entity.Null;
                EnemyComponent closestValidEnemy = default;
                float radius = ability.range;
                bool foundEnemyInRadius = false;
                int gridSize = gridComponent.size.x;

                CheckPosition(playerGridPosition, ref closestEnemyEntity, ref closestValidEnemy,
                    ref foundEnemyInRadius);

                for (int i = 1; i < radius; i++)
                {
                    if (foundEnemyInRadius) break;

                    for (int j = 0; j < Directions.Length; j++)
                    {
                        int2 newGridDirection = new int2(Directions[j].x * i, Directions[j].y * i);

                        int2 gridPosition = playerGridPosition + newGridDirection;

                        if (gridPosition.x > gridSize || gridPosition.y > gridSize || gridPosition.x < -gridSize ||
                            gridPosition.y < -gridSize) continue;

                        CheckPosition(gridPosition, ref closestEnemyEntity, ref closestValidEnemy,
                            ref foundEnemyInRadius);

                        if (foundEnemyInRadius) break;
                    }
                }

                if (!foundEnemyInRadius) return;

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
                    Position = playerComponent.position,
                    Rotation = quaternion.identity,
                    Scale = ability.projectileScale
                });

                ecb.AddComponent(sortKey, projectileEntity, new TargetComponent
                {
                    enemy = closestValidEnemy,
                    enemyEntity = closestEnemyEntity
                });
            }

            ability.cooldownRemaining = ability.cooldown;
        }
        else
        {
            ability.cooldownRemaining -= deltaTime;
        }
    }

    [BurstCompile]
    private void CheckPosition(int2 gridPosition, ref Entity closestEnemyEntity, ref EnemyComponent closestValidEnemy,
        ref bool foundEnemyInRadius)
    {
        if (gridComponent.enemyPositions.TryGetFirstValue(gridPosition, out Entity enemyEntity,
                out NativeParallelMultiHashMapIterator<int2> iterator))
        {
            do
            {
                if (enemyLookup.HasComponent(enemyEntity))
                {
                    EnemyComponent enemy = enemyLookup[enemyEntity];

                    if (enemy.isFullySpawned == 0) continue;

                    closestEnemyEntity = enemyEntity;
                    closestValidEnemy = enemy;
                    foundEnemyInRadius = true;
                }
            } while (gridComponent.enemyPositions.TryGetNextValue(out enemyEntity, ref iterator));
        }
    }
}
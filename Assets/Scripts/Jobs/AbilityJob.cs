using Components;
using Helpers;
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

    private void Execute([ChunkIndexInQuery] int sortKey, ref AbilityComponent ability, in Entity entity)
    {
        if (ability.cooldownRemaining <= 0f)
        {
            if (ability.hasProjectile == 1)
            {
                int2 playerGridPosition = playerComponent.gridPosition;
                Entity closestEnemyEntity = Entity.Null;
                EnemyComponent closestValidEnemy = default;
                bool foundEnemyInRadius = false;
                int gridSize = gridComponent.size.x;

                if (ability.positionsToCheck.Equals(default)) GetPositions(ref ability);

                for (int i = 0; i < ability.positionsToCheck.Value.positions.Length; i++)
                {
                    int2 positionToCheck = ability.positionsToCheck.Value.positions[i];
                    int2 gridPosition = playerGridPosition + positionToCheck;

                    if (gridPosition.x > gridSize || gridPosition.y > gridSize || gridPosition.x < -gridSize ||
                        gridPosition.y < -gridSize) continue;

                    CheckPosition(gridPosition, ref closestEnemyEntity, ref closestValidEnemy,
                        ref foundEnemyInRadius);

                    if (foundEnemyInRadius) break;
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
    private void GetPositions(ref AbilityComponent ability)
    {
        int maxDistance = ability.range;
        int numberOfPositions = 1 + 8 * maxDistance * (maxDistance + 1) / 2;

        NativeArray<int2> positions = new NativeArray<int2>(numberOfPositions, Allocator.Temp);

        int i = 0;
        positions[i] = new int2(0, 0);
        i++;

        for (int d = 1; d <= maxDistance; d++)
        {
            // Top edge: from (-d, d) to (d, d)
            for (int x = -d; x <= d; x++)
            {
                positions[i] = new int2(x, d);
                i++;
            }

            // Right edge: from (d, d - 1) to (d, -d)
            for (int y = d - 1; y >= -d; y--)
            {
                positions[i] = new int2(d, y);
                i++;
            }

            // Bottom edge: from (d - 1, -d) to (-d, -d)
            for (int x = d - 1; x >= -d; x--)
            {
                positions[i] = new int2(x, -d);
                i++;
            }

            // Left edge: from (-d, -d + 1) to (-d, d - 1)
            for (int y = -d + 1; y <= d - 1; y++)
            {
                positions[i] = new int2(-d, y);
                i++;
            }
        }

        BlobUtility.CreateAndAssignPositionsBlob(ref positions, ref ability);
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

                    return;
                }
            } while (gridComponent.enemyPositions.TryGetNextValue(out enemyEntity, ref iterator));
        }
    }
}
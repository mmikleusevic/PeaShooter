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
    [ReadOnly] public ComponentLookup<EnemyComponent> enemyComponentLookup;
    [ReadOnly] public PlayerComponent playerComponent;
    [ReadOnly] public float deltaTime;

    private void Execute([ChunkIndexInQuery] int sortKey, ref AbilityComponent abilityComponent,
        in Entity abilityEntity)
    {
        if (abilityComponent.cooldownRemaining <= 0f)
        {
            if (abilityComponent.hasProjectile == 1)
            {
                int2 playerGridPosition = playerComponent.gridPosition;
                Entity closestEnemyEntity = Entity.Null;
                EnemyComponent closestValidEnemy = default;
                bool foundEnemyInRadius = false;

                if (!abilityComponent.positionsToCheck.IsCreated) GetPositions(ref abilityComponent);

                for (int i = 0; i < abilityComponent.positionsToCheck.Value.positions.Length; i++)
                {
                    int2 positionToCheck = abilityComponent.positionsToCheck.Value.positions[i];
                    int2 gridPosition = playerGridPosition + positionToCheck;

                    if (!gridComponent.enemyPositions.ContainsKey(gridPosition)) continue;

                    CheckPosition(gridPosition, ref closestEnemyEntity, ref closestValidEnemy,
                        ref foundEnemyInRadius);

                    if (foundEnemyInRadius) break;
                }

                if (!foundEnemyInRadius) return;

                if (projectileEntity == Entity.Null)
                {
                    projectileEntity = ecb.Instantiate(sortKey, abilityComponent.abilityEntity);

                    ecb.SetName(sortKey, projectileEntity, "Projectile");

                    ecb.AddComponent(sortKey, projectileEntity, new ProjectileAbilityComponent
                    {
                        parentEntity = abilityEntity
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
                    Scale = abilityComponent.scale
                });

                ecb.AddComponent(sortKey, projectileEntity, new TargetComponent
                {
                    enemyComponent = closestValidEnemy,
                    enemyEntity = closestEnemyEntity
                });
            }

            abilityComponent.cooldownRemaining = abilityComponent.cooldown;
        }
        else
        {
            abilityComponent.cooldownRemaining -= deltaTime;
        }
    }

    [BurstCompile]
    private void GetPositions(ref AbilityComponent abilityComponent)
    {
        int maxDistance = abilityComponent.range;
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

        BlobUtility.CreateAndAssignPositionsBlob(ref positions, ref abilityComponent);
    }

    [BurstCompile]
    private void CheckPosition(int2 gridPosition, ref Entity closestEnemyEntity,
        ref EnemyComponent closestValidEnemyComponent,
        ref bool foundEnemyInRadius)
    {
        if (gridComponent.enemyPositions.TryGetFirstValue(gridPosition, out Entity enemyEntity,
                out NativeParallelMultiHashMapIterator<int2> iterator))
        {
            do
            {
                if (enemyComponentLookup.HasComponent(enemyEntity))
                {
                    RefRO<EnemyComponent> enemy = enemyComponentLookup.GetRefRO(enemyEntity);

                    if (enemy.ValueRO.isFullySpawned == 0) continue;

                    closestEnemyEntity = enemyEntity;
                    closestValidEnemyComponent = enemy.ValueRO;
                    foundEnemyInRadius = true;

                    return;
                }
            } while (gridComponent.enemyPositions.TryGetNextValue(out enemyEntity, ref iterator));
        }
    }
}
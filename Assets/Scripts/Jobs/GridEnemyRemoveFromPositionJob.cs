using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Jobs
{
    [BurstCompile]
    public struct GridEnemyRemoveFromPositionJob : IJob
    {
        public NativeParallelMultiHashMap<int2, Entity> enemyPositions;
        public NativeParallelMultiHashMap<int2, Entity> entitiesOnPositionsToRemove;

        public void Execute()
        {
            if (entitiesOnPositionsToRemove.IsEmpty) return;

            NativeArray<int2> positions = entitiesOnPositionsToRemove.GetKeyArray(Allocator.Temp);
            NativeParallelHashSet<PositionEntityPairComponent> entitiesToReAdd =
                new NativeParallelHashSet<PositionEntityPairComponent>(enemyPositions.Count(), Allocator.Temp);
            NativeArray<Entity> removalEntities = entitiesOnPositionsToRemove.GetValueArray(Allocator.Temp);

            foreach (int2 position in positions)
            {
                if (enemyPositions.TryGetFirstValue(position, out Entity foundEntity, out var iterator))
                {
                    do
                    {
                        if (!removalEntities.Contains(foundEntity))
                        {
                            entitiesToReAdd.Add(new PositionEntityPairComponent
                            {
                                position = position,
                                entity = foundEntity
                            });
                        }
                    } while (enemyPositions.TryGetNextValue(out foundEntity, ref iterator));
                }

                enemyPositions.Remove(position);
            }

            foreach (PositionEntityPairComponent entityPosition in entitiesToReAdd)
            {
                enemyPositions.Add(entityPosition.position, entityPosition.entity);
            }

            positions.Dispose();
            entitiesToReAdd.Dispose();
            removalEntities.Dispose();

            entitiesOnPositionsToRemove.Clear();
        }
    }
}
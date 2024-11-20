using Components;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Jobs
{
    [BurstCompile]
    public struct GridAddEnemyToPositionFlagItJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter ecb;

        public NativeParallelMultiHashMap<int2, Entity>.ParallelWriter enemyPositionsWriter;
        public NativeParallelMultiHashMap<int2, Entity>.ParallelWriter positionRemovalsParallel;

        [ReadOnly] public ComponentTypeHandle<GridEnemyPositionUpdateComponent> gridEnemyPositionUpdateTypeHandle;
        [ReadOnly] public EntityTypeHandle entityTypeHandle;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
            in v128 chunkEnabledMask)
        {
            NativeArray<GridEnemyPositionUpdateComponent> enemyPositionUpdates =
                chunk.GetNativeArray(ref gridEnemyPositionUpdateTypeHandle);
            NativeArray<Entity> entities = chunk.GetNativeArray(entityTypeHandle);

            for (int i = 0; i < chunk.Count; i++)
            {
                GridEnemyPositionUpdateComponent enemyPositionUpdate = enemyPositionUpdates[i];

                Entity entity = entities[i];

                if (enemyPositionUpdate.status == UpdateStatus.Add)
                {
                    enemyPositionsWriter.Add(enemyPositionUpdate.position, entity);
                }
                else if (enemyPositionUpdate.status == UpdateStatus.Move)
                {
                    enemyPositionsWriter.Add(enemyPositionUpdate.position, entity);
                    positionRemovalsParallel.Add(enemyPositionUpdate.oldPosition, entity);
                }
                else if (enemyPositionUpdate.status == UpdateStatus.Remove)
                {
                    positionRemovalsParallel.Add(enemyPositionUpdate.oldPosition, entity);
                }

                ecb.RemoveComponent<PositionChangedComponent>(i, entity);
            }
        }
    }
}
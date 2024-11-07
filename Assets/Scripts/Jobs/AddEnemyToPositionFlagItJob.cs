using Components;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Jobs
{
    [BurstCompile]
    public struct AddEnemyToPositionFlagItJob : IJobChunk
    {
        public EntityCommandBuffer.ParallelWriter ecb;

        public NativeParallelMultiHashMap<int2, Entity>.ParallelWriter enemyPositionsWriter;
        public NativeParallelMultiHashMap<int2, Entity>.ParallelWriter removalPositionsParallel;

        [ReadOnly] public ComponentTypeHandle<GridEnemyPositionUpdateComponent> GridEnemyPositionUpdateTypeHandle;
        [ReadOnly] public EntityTypeHandle EntityTypeHandle;

        public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
            in v128 chunkEnabledMask)
        {
            NativeArray<GridEnemyPositionUpdateComponent> enemyPositionUpdates =
                chunk.GetNativeArray(ref GridEnemyPositionUpdateTypeHandle);
            NativeArray<Entity> entities = chunk.GetNativeArray(EntityTypeHandle);

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
                    removalPositionsParallel.Add(enemyPositionUpdate.oldPosition, entity);
                }
                else if (enemyPositionUpdate.status == UpdateStatus.Remove)
                {
                    removalPositionsParallel.Add(enemyPositionUpdate.oldPosition, entity);
                }

                ecb.RemoveComponent<HasChangedPositionComponent>(i, entity);
            }
        }
    }
}
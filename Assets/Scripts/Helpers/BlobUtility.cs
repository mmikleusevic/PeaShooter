using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Helpers
{
    [BurstCompile]
    public static class BlobUtility
    {
        [BurstCompile]
        public static void CreateAndAssignPositionsBlob(ref NativeArray<int2> positions, ref AbilityComponent ability)
        {
            using (BlobBuilder builder = new BlobBuilder(Allocator.Temp))
            {
                ref PositionsComponent positionsComponent = ref builder.ConstructRoot<PositionsComponent>();

                BlobBuilderArray<int2> positionsArray =
                    builder.Allocate(ref positionsComponent.positions, positions.Length);

                for (int i = 0; i < positions.Length; i++)
                {
                    positionsArray[i] = positions[i];
                }

                ability.positionsToCheck = builder.CreateBlobAssetReference<PositionsComponent>(Allocator.Persistent);
            }

            positions.Dispose();
        }
    }
}
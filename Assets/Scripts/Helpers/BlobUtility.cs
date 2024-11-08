using Game;
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
                ref PositionsBlob positionsBlob = ref builder.ConstructRoot<PositionsBlob>();

                BlobBuilderArray<int2> positionsArray = builder.Allocate(ref positionsBlob.positions, positions.Length);

                for (int i = 0; i < positions.Length; i++)
                {
                    positionsArray[i] = positions[i];
                }

                ability.positionsToCheck = builder.CreateBlobAssetReference<PositionsBlob>(Allocator.Temp);
            }

            positions.Dispose();
        }
    }
}
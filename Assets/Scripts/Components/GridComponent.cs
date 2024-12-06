using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct GridComponent : IComponentData, IDisposable
    {
        public NativeHashMap<int2, byte> gridNodes;
        public NativeParallelMultiHashMap<int2, Entity> enemyPositions;

        public int2 size;

        public void Dispose()
        {
            if (enemyPositions.IsCreated)
            {
                enemyPositions.Dispose();
            }

            if (gridNodes.IsCreated)
            {
                gridNodes.Dispose();
            }
        }
    }
}
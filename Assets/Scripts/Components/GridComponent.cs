using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct GridComponent : IComponentData
{
    public NativeHashMap<int2, byte> gridNodes;
    public NativeParallelMultiHashMap<int2, Entity> enemyPositions;

    public int2 size;
}
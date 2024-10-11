using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct GridComponent : IComponentData
{
    public NativeHashMap<int2, byte> gridNodes;
    public NativeHashMap<int2, NativeList<Entity>> enemyPositions;

    public int2 size;
}
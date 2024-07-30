using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct GridComponent : IComponentData
{
    public NativeHashMap<int2, bool> gridNodes;
}
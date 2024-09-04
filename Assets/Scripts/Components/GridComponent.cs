using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
public struct GridComponent : IComponentData
{
    public NativeHashMap<int2, bool> gridNodes;

    public int2 size;

    public bool IsValidPosition(int2 position)
    {
        if (!gridNodes.ContainsKey(position)) return false;

        return gridNodes[position];
    }
}
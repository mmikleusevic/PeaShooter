using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
public struct GridComponent : IComponentData
{
    public NativeHashMap<int2, byte> gridNodes;

    public int2 size;

    public byte IsValidPosition(int2 position)
    {
        if (!gridNodes.ContainsKey(position)) return 0;

        return gridNodes[position];
    }
}
using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(16)]
public struct NodeComponent : IBufferElementData
{
    public int2 position;
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;
}
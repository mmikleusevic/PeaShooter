using Unity.Entities;
using Unity.Mathematics;

public struct Node : IBufferElementData
{
    public float2 position;
    public float gCost;
    public float hCost;
    public float fCost => gCost + hCost;

    public int parentIndex;
}
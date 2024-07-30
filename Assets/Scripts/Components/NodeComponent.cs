using Unity.Entities;
using Unity.Mathematics;

public struct Node : IBufferElementData
{
    public int2 position;
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;
    private bool isWalkbable;
    public bool IsWalkable
    {
        get { return isWalkbable; }
        set { isWalkbable = value; }
    }

    public int lastNodeIndex;
}
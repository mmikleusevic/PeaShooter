using Unity.Entities;
using Unity.Mathematics;

public struct Node : IComponentData
{
    public float2 position;
    public float gCost;
    public float hCost;
    public Entity parent;
    public float fCost => gCost + hCost;
}
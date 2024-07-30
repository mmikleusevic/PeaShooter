using Unity.Entities;
using Unity.Mathematics;

public struct RandomDataComponent : IComponentData
{
    public Random seed;

    public int2 minimumPosition;
    public int2 maximumPosition;
    public int2 nextPosition => seed.NextInt2(minimumPosition, maximumPosition);
}
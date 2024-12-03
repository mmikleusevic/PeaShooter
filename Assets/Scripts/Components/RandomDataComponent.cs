using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

public struct RandomDataComponent : IComponentData
{
    public Random seed;

    public int2 minimumPosition;
    public int2 maximumPosition;
    private int2 nextPosition => seed.NextInt2(minimumPosition, maximumPosition);

    public int2 GetRandomPosition(NativeHashMap<int2, byte> gridNodes)
    {
        int2 newPosition;

        do
        {
            newPosition = nextPosition;
        } while (gridNodes[newPosition] == 0 || newPosition.Equals(default));

        return newPosition;
    }
}
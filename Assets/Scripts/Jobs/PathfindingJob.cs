using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
partial struct PathfindingJob : IJobEntity
{
    [ReadOnly] private const int STRAIGHT_COST = 10;
    [ReadOnly] private const int DIAGONAL_COST = 14;
    [ReadOnly] public int2 playerPosition;
    [ReadOnly] public GridComponent grid;

    public void Execute(ref EnemyComponent enemy, ref DynamicBuffer<NodeComponent> pathBuffer)
    {
        pathBuffer.Clear();

        NativeList<int2> path = FindPath(enemy.gridPosition, playerPosition);

        if (path.IsCreated)
        {
            foreach (var node in path)
            {
                pathBuffer.Add(new NodeComponent { position = node });
            }

            enemy.currentPathIndex = 0;
        }

        path.Dispose();
    }

    [BurstCompile]
    private NativeList<int2> FindPath(int2 start, int2 goal)
    {
        var result = new NativeList<int2>(Allocator.Temp);

        if (start.Equals(goal)) return result;

        var openSet = new NativeList<NodeComponent>(Allocator.Temp);
        var closedSet = new NativeHashSet<int2>(10, Allocator.Temp);
        var cameFrom = new NativeHashMap<int2, NodeComponent>(100, Allocator.Temp);

        openSet.Add(new NodeComponent { position = start, gCost = 0, hCost = CalculateDistanceCost(start, goal) });

        while (openSet.Length > 0)
        {
            int currentIndex = GetLowestFCostIndex(openSet);
            var current = openSet[currentIndex];

            if (current.position.Equals(goal))
            {
                ReconstructPath(result, cameFrom, current.position);
                break;
            }

            openSet.RemoveAtSwapBack(currentIndex);
            closedSet.Add(current.position);

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;

                    if (math.abs(x) == 1 && math.abs(y) == 1)
                    {
                        if (!grid.IsValidPosition(current.position + new int2(x, 0)) || !grid.IsValidPosition(current.position + new int2(0, y))) continue;
                    }

                    int2 neighborPos = current.position + new int2(x, y);

                    if (closedSet.Contains(neighborPos) || !grid.IsValidPosition(neighborPos)) continue;

                    int tentativeGCost = current.gCost + CalculateDistanceCost(current.position, neighborPos);

                    bool inOpenSet = false;
                    for (int i = 0; i < openSet.Length; i++)
                    {
                        if (openSet[i].position.Equals(neighborPos))
                        {
                            inOpenSet = true;
                            if (tentativeGCost < openSet[i].gCost)
                            {
                                openSet[i] = new NodeComponent
                                {
                                    position = neighborPos,
                                    gCost = tentativeGCost,
                                    hCost = CalculateDistanceCost(neighborPos, goal)
                                };

                                cameFrom[neighborPos] = current;
                            }
                            break;
                        }
                    }

                    if (!inOpenSet)
                    {
                        openSet.Add(new NodeComponent
                        {
                            position = neighborPos,
                            gCost = tentativeGCost,
                            hCost = CalculateDistanceCost(neighborPos, goal),
                        });

                        cameFrom[neighborPos] = current;
                    }
                }
            }
        }

        openSet.Dispose();
        closedSet.Dispose();
        cameFrom.Dispose();

        return result;
    }

    [BurstCompile]
    private int CalculateDistanceCost(int2 from, int2 to)
    {
        int xDistance = math.abs(from.x - to.x);
        int yDistance = math.abs(from.y - to.y);
        int remaining = math.abs(xDistance - yDistance);

        return DIAGONAL_COST * math.min(xDistance, yDistance) + STRAIGHT_COST * remaining;
    }

    [BurstCompile]
    private int GetLowestFCostIndex(NativeList<NodeComponent> openSet)
    {
        int lowestIndex = 0;
        int lowestFCost = int.MaxValue;

        for (int i = 0; i < openSet.Length; i++)
        {
            if (openSet[i].fCost < lowestFCost)
            {
                lowestFCost = openSet[i].fCost;
                lowestIndex = i;
            }
        }

        return lowestIndex;
    }

    [BurstCompile]
    private void ReconstructPath(NativeList<int2> result, NativeHashMap<int2, NodeComponent> cameFrom, int2 current)
    {
        NativeList<int2> tempPath = new NativeList<int2>(Allocator.Temp);

        while (cameFrom.ContainsKey(current))
        {
            tempPath.Add(current);
            current = cameFrom[current].position;
        }

        tempPath.Add(current);

        for (int i = tempPath.Length - 1; i >= 0; i--)
        {
            result.Add(tempPath[i]);
        }

        tempPath.Dispose();
    }
}
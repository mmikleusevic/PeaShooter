using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
partial struct PathfindingJob : IJobEntity
{
    [ReadOnly] public int2 playerPosition;
    [ReadOnly] public GridComponent grid;

    public void Execute(ref EnemyComponent enemy, ref DynamicBuffer<Node> pathBuffer)
    {
        pathBuffer.Clear();

        NativeList<int2> path = FindPath(enemy.position, playerPosition);

        if (path.IsCreated)
        {
            foreach (var node in path)
            {
                pathBuffer.Add(new Node { position = node });
            }

            enemy.currentPathIndex = 0;
        }

        path.Dispose();
    }

    [BurstCompile]
    private NativeList<int2> FindPath(int2 start, int2 goal)
    {
        var result = new NativeList<int2>(Allocator.Temp);
        var openSet = new NativeList<Node>(Allocator.Temp);
        var closedSet = new NativeHashSet<int2>(10, Allocator.Temp);
        var cameFrom = new NativeHashMap<int2, Node>(100, Allocator.Temp);

        openSet.Add(new Node { position = start, gCost = 0, hCost = CalculateHCost(start, goal) });

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
                for (int z = -1; z <= 1; z++)
                {
                    if (x == 0 && z == 0) continue;

                    int2 neighborPos = current.position + new int2(x, z);

                    if (closedSet.Contains(neighborPos) || !grid.IsValidPosition(neighborPos)) continue;

                    int tentativeGCost = current.gCost + 1;

                    bool inOpenSet = false;
                    for (int i = 0; i < openSet.Length; i++)
                    {
                        if (openSet[i].position.Equals(neighborPos))
                        {
                            inOpenSet = true;
                            if (tentativeGCost < openSet[i].gCost)
                            {
                                openSet[i] = new Node
                                {
                                    position = neighborPos,
                                    gCost = tentativeGCost,
                                    hCost = CalculateHCost(neighborPos, goal)
                                };

                                cameFrom[neighborPos] = current;
                            }
                            break;
                        }
                    }

                    if (!inOpenSet)
                    {
                        openSet.Add(new Node
                        {
                            position = neighborPos,
                            gCost = tentativeGCost,
                            hCost = CalculateHCost(neighborPos, goal),
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
    private int CalculateHCost(int2 from, int2 to)
    {
        return math.abs(from.x - to.x) + math.abs(from.y - to.y); // Manhattan distance
    }

    [BurstCompile]
    private int GetLowestFCostIndex(NativeList<Node> openSet)
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
    private void ReconstructPath(NativeList<int2> result, NativeHashMap<int2, Node> cameFrom, int2 current)
    {
        while (cameFrom.ContainsKey(current))
        {
            result.Add(current);
            current = cameFrom[current].position;
        }

        result.Add(current);
        ReverseList(ref result);
    }

    [BurstCompile]
    private void ReverseList(ref NativeList<int2> list)
    {
        int count = list.Length;
        for (int i = 0; i < count / 2; i++)
        {
            int2 temp = list[i];
            list[i] = list[count - 1 - i];
            list[count - 1 - i] = temp;
        }
    }
}
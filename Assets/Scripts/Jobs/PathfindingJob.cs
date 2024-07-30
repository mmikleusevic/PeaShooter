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

            path.Dispose();
            enemy.currentPathIndex = 0;
        }
    }

    [BurstCompile]
    private NativeList<int2> FindPath(int2 start, int2 goal)
    {
        var result = new NativeList<int2>(Allocator.Temp);
        var openSet = new NativeList<Node>(Allocator.Temp);
        var closedSet = new NativeHashSet<int2>(10, Allocator.Temp);
        var nodeMap = new NativeHashMap<int2, Node>(100, Allocator.Temp);

        openSet.Add(new Node { position = start, gCost = 0, hCost = CalculateHCost(start, goal) });
        nodeMap.Add(start, openSet[0]);

        while (openSet.Length > 0)
        {
            int currentIndex = GetLowestFCostIndex(openSet);
            var current = openSet[currentIndex];

            if (math.distancesq(current.position, goal) < 0.01f)
            {
                ReconstructPath(result, nodeMap, current.position);
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
                    if (closedSet.Contains(neighborPos) || !grid.gridNodes[neighborPos]) continue;

                    int tentativeGCost = current.gCost + (int)math.distance(current.position, neighborPos);

                    Node neighbor;

                    if (!nodeMap.TryGetValue(neighborPos, out neighbor))
                    {
                        neighbor = new Node
                        {
                            position = neighborPos,
                            gCost = tentativeGCost,
                            hCost = CalculateHCost(neighborPos, goal),
                            lastNodeIndex = nodeMap[current.position].lastNodeIndex
                        };
                        openSet.Add(neighbor);
                        nodeMap.Add(neighborPos, neighbor);
                    }
                    else if (tentativeGCost < neighbor.gCost)
                    {
                        neighbor.gCost = tentativeGCost;
                        neighbor.lastNodeIndex = nodeMap[current.position].lastNodeIndex;
                        nodeMap[neighborPos] = neighbor;
                    }
                }
            }
        }

        openSet.Dispose();
        closedSet.Dispose();
        nodeMap.Dispose();

        return result;
    }

    [BurstCompile]
    private int CalculateHCost(int2 from, int2 to)
    {
        return (int)math.distance(from, to);
    }

    [BurstCompile]
    private int GetLowestFCostIndex(NativeList<Node> openSet)
    {
        int lowestIndex = 0;
        float lowestFCost = float.MaxValue;

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
    private void ReconstructPath(NativeList<int2> result, NativeHashMap<int2, Node> nodeMap, int2 current)
    {
        while (nodeMap.ContainsKey(current))
        {
            result.Add(current);
            current = nodeMap[current].position;
        }

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
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
partial struct PathfindingJob : IJobEntity
{
    [ReadOnly] public float2 playerPosition;
    [ReadOnly] public NativeArray<ObstacleComponent> obstacles;

    public void Execute(ref EnemyComponent enemy, ref DynamicBuffer<Node> pathBuffer)
    {
        pathBuffer.Clear();
        NativeList<float2> path = FindPath(enemy.position, playerPosition);

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
    private NativeList<float2> FindPath(float2 start, float2 goal)
    {
        var result = new NativeList<float2>(Allocator.Temp);
        var openSet = new NativeList<Node>(Allocator.Temp);
        var closedSet = new NativeHashSet<float2>(10, Allocator.Temp);
        var nodeMap = new NativeHashMap<float2, Node>(100, Allocator.Temp);

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

                    float2 neighborPos = current.position + new float2(x, z);
                    if (closedSet.Contains(neighborPos) || !IsWalkable(neighborPos)) continue;

                    float tentativeGCost = current.gCost + math.distance(current.position, neighborPos);

                    Node neighbor;
                    if (!nodeMap.TryGetValue(neighborPos, out neighbor))
                    {
                        neighbor = new Node
                        {
                            position = neighborPos,
                            gCost = tentativeGCost,
                            hCost = CalculateHCost(neighborPos, goal),
                            parentIndex = nodeMap[current.position].parentIndex
                        };
                        openSet.Add(neighbor);
                        nodeMap.Add(neighborPos, neighbor);
                    }
                    else if (tentativeGCost < neighbor.gCost)
                    {
                        neighbor.gCost = tentativeGCost;
                        neighbor.parentIndex = nodeMap[current.position].parentIndex;
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
    private float CalculateHCost(float2 from, float2 to)
    {
        return math.distance(from, to);
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
    private void ReconstructPath(NativeList<float2> result, NativeHashMap<float2, Node> nodeMap, float2 current)
    {
        while (nodeMap.ContainsKey(current))
        {
            result.Add(current);
            current = nodeMap[current].position;
        }

        ReverseList(ref result);
    }

    [BurstCompile]
    private bool IsWalkable(float2 position)
    {
        foreach (var obstacle in obstacles)
        {
            if (math.distancesq(position, obstacle.position) <= obstacle.size.x * obstacle.size.y)
            {
                return false;
            }
        }

        return true;
    }

    [BurstCompile]
    private void ReverseList(ref NativeList<float2> list)
    {
        int count = list.Length;
        for (int i = 0; i < count / 2; i++)
        {
            float2 temp = list[i];
            list[i] = list[count - 1 - i];
            list[count - 1 - i] = temp;
        }
    }
}
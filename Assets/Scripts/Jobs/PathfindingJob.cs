using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
internal partial struct PathfindingJob : IJobEntity
{
    [ReadOnly] private const int STRAIGHT_COST = 10;
    [ReadOnly] private const int DIAGONAL_COST = 14;
    [ReadOnly] public float elapsedTime;
    [ReadOnly] public float defaultMoveSpeed;
    [ReadOnly] public int2 playerPosition;
    [ReadOnly] public NativeHashMap<int2, byte> gridNodes;
    [ReadOnly] public InputComponent inputComponent;

    private static readonly int2[] directions =
    {
        new(-1, 1), new(0, 1), new(1, 1),
        new(-1, 0), new(1, 0),
        new(-1, -1), new(0, -1), new(1, -1)
    };

    private void Execute(ref EnemyComponent enemyComponent, ref DynamicBuffer<NodeComponent> pathBuffer)
    {
        float timeOfNextPathfinding = math.min(0.5f, defaultMoveSpeed / enemyComponent.moveSpeed) +
                                      enemyComponent.timeOfLastPathfinding;

        if (enemyComponent.isFullySpawned == 0 || elapsedTime < timeOfNextPathfinding) return;

        enemyComponent.timeOfLastPathfinding = elapsedTime;

        pathBuffer.Clear();

        int2 predictedPlayerPosition = playerPosition + (int2)math.round(inputComponent.moveInput);

        if (IsValidPosition(predictedPlayerPosition) == 0) predictedPlayerPosition = playerPosition;

        NativeList<int2> path = FindPath(enemyComponent.gridPosition, predictedPlayerPosition);

        if (path.IsCreated)
        {
            foreach (int2 node in path) pathBuffer.Add(new NodeComponent { position = node });

            enemyComponent.currentPathIndex = 0;
        }

        path.Dispose();
    }

    [BurstCompile]
    private NativeList<int2> FindPath(int2 start, int2 goal)
    {
        NativeList<int2> result = new NativeList<int2>(Allocator.Temp);

        if (start.Equals(goal)) return result;

        float distance = math.distance(start, goal);
        int estimatedNodes = (int)((distance + 1) * 1.5f);
        NativeList<NodeComponent> openSet = new NativeList<NodeComponent>(estimatedNodes, Allocator.Temp);
        NativeHashSet<int2> closedSet = new NativeHashSet<int2>(estimatedNodes, Allocator.Temp);
        NativeHashMap<int2, int2> cameFrom = new NativeHashMap<int2, int2>(estimatedNodes, Allocator.Temp);

        openSet.Add(new NodeComponent { position = start, gCost = 0, hCost = CalculateDistanceCost(start, goal) });

        while (openSet.Length > 0)
        {
            int currentIndex = GetLowestFCostIndex(openSet);
            NodeComponent currentNodeComponent = openSet[currentIndex];

            if (currentNodeComponent.position.Equals(goal))
            {
                ReconstructPath(result, cameFrom, currentNodeComponent.position);
                break;
            }

            openSet.RemoveAtSwapBack(currentIndex);
            closedSet.Add(currentNodeComponent.position);

            for (int i = 0; i < directions.Length; i++)
            {
                if (i == 0 || i == 2 || i == 5 || i == 7)
                {
                    int2 dirX = new int2(directions[i].x, 0);
                    int2 dirY = new int2(0, directions[i].y);
                    if (IsValidPosition(currentNodeComponent.position + dirX) == 0 ||
                        IsValidPosition(currentNodeComponent.position + dirY) == 0)
                        continue;
                }

                int2 neighborPos = currentNodeComponent.position + directions[i];

                if (closedSet.Contains(neighborPos) || IsValidPosition(neighborPos) == 0) continue;

                int tentativeGCost = currentNodeComponent.gCost +
                                     CalculateDistanceCost(currentNodeComponent.position, neighborPos);

                bool inOpenSet = false;
                for (int j = 0; j < openSet.Length; j++)
                    if (openSet[j].position.Equals(neighborPos))
                    {
                        inOpenSet = true;
                        if (tentativeGCost < openSet[j].gCost)
                        {
                            openSet[j] = new NodeComponent
                            {
                                position = neighborPos,
                                gCost = tentativeGCost,
                                hCost = CalculateDistanceCost(neighborPos, goal)
                            };

                            cameFrom[neighborPos] = currentNodeComponent.position;
                        }

                        break;
                    }

                if (!inOpenSet)
                {
                    openSet.Add(new NodeComponent
                    {
                        position = neighborPos,
                        gCost = tentativeGCost,
                        hCost = CalculateDistanceCost(neighborPos, goal)
                    });

                    cameFrom[neighborPos] = currentNodeComponent.position;
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
            if (openSet[i].fCost < lowestFCost)
            {
                lowestFCost = openSet[i].fCost;
                lowestIndex = i;
            }

        return lowestIndex;
    }

    [BurstCompile]
    private void ReconstructPath(NativeList<int2> result, NativeHashMap<int2, int2> cameFrom, int2 current)
    {
        int pathLength = 0;
        int2 tempCurrent = current;

        while (cameFrom.ContainsKey(tempCurrent))
        {
            pathLength++;
            tempCurrent = cameFrom[tempCurrent];
        }

        result.ResizeUninitialized(pathLength + 1);

        int index = pathLength;
        while (cameFrom.ContainsKey(current))
        {
            result[index--] = current;
            current = cameFrom[current];
        }

        result[0] = current;
    }

    [BurstCompile]
    private byte IsValidPosition(int2 position)
    {
        return !gridNodes.ContainsKey(position) ? (byte)0 : gridNodes[position];
    }
}
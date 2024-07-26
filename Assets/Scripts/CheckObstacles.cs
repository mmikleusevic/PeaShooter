using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
public struct CheckObstacles
{
    [BurstCompile]
    public static void GetValidPosition(in NativeList<float3> obstaclePositions, ref RandomDataComponent randomData, in float distance, ref float3 candidatePosition)
    {
        if (obstaclePositions.Length == 0)
        {
            candidatePosition = randomData.nextPosition;
            return;
        }

        while (true)
        {
            candidatePosition = randomData.nextPosition;

            if (IsPositionValid(obstaclePositions, candidatePosition, distance))
            {
                return;
            }
        }
    }

    [BurstCompile]
    public static void GetValidPosition(in NativeList<ObstacleComponent> obstacles, ref RandomDataComponent randomData, in float distance, ref float3 candidatePosition)
    {
        if (obstacles.Length == 0)
        {
            candidatePosition = randomData.nextPosition;
            return;
        }

        while (true)
        {
            candidatePosition = randomData.nextPosition;

            if (IsPositionValid(obstacles, candidatePosition, distance))
            {
                return;
            }
        }
    }

    [BurstCompile]
    private static bool IsPositionValid(in NativeList<float3> obstaclePositions, in float3 candidatePosition, in float distance)
    {
        foreach (float3 occupiedPosition in obstaclePositions)
        {
            if (MathExtensions.AreTooClose(occupiedPosition, candidatePosition, distance))
            {
                return false;
            }
        }

        return true;
    }

    [BurstCompile]
    private static bool IsPositionValid(in NativeList<ObstacleComponent> obstacles, in float3 candidatePosition, in float distance)
    {
        foreach (ObstacleComponent occupiedPosition in obstacles)
        {
            if (MathExtensions.AreTooClose(new float3(occupiedPosition.position.x, 0, occupiedPosition.position.y), candidatePosition, distance))
            {
                return false;
            }
        }

        return true;
    }
}
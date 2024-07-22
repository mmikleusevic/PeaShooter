using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct PathfindingJob : IJobEntity
{
    [ReadOnly] public float3 position;
    [ReadOnly] public float3 playerPosition;

    public void Execute(ref PathfindingComponent pathfindingComponent)
    {
        pathfindingComponent.startPosition = position;
        pathfindingComponent.endPosition = playerPosition;
    }
}
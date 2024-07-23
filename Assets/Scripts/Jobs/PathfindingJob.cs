using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct PathfindingJob : IJobEntity
{
    [ReadOnly] public float2 position;
    [ReadOnly] public float2 playerPosition;

    public void Execute(ref PathfindingRequestComponent pathfindingComponent)
    {
        pathfindingComponent.start = position;
        pathfindingComponent.end = playerPosition;
    }
}
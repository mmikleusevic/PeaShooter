using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ObstacleUpdateJob : IJobEntity
{
    public void Execute(ref ObstacleComponent obstacleComponent, in LocalTransform transform)
    {
        obstacleComponent.position = new float2(transform.Position.x, transform.Position.z);
    }
}
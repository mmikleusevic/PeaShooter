using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct ObstacleUpdateJob : IJobEntity
{
    public DynamicBuffer<ObstacleBuffer> buffer;

    public void Execute(ref ObstacleComponent obstacleComponent, in LocalTransform transform)
    {
        obstacleComponent.position = new float2(transform.Position.x, transform.Position.z);

        buffer.Add(new ObstacleBuffer
        {
            value = obstacleComponent
        });
    }
}
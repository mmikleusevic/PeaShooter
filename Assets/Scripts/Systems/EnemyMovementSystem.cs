using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct EnemyMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (enemy, transform) in SystemAPI.Query<RefRO<EnemyComponent>, RefRW<LocalTransform>>())
        {
            float3 currentPosition = transform.ValueRO.Position;

            currentPosition.x = math.clamp(currentPosition.x, -Config.Instance.GetPlaneSize(), Config.Instance.GetPlaneSize());
            currentPosition.y = 0;
            currentPosition.z = math.clamp(currentPosition.z, -Config.Instance.GetPlaneSize(), Config.Instance.GetPlaneSize());

            transform.ValueRW.Position = currentPosition;
        }
    }
}

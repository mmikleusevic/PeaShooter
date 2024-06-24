using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct EnemyMovementSystem : ISystem
{
    private float3 lastDirection;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (enemy, transform, physics) in SystemAPI.Query<RefRO<EnemyComponent>, RefRW<LocalTransform>, RefRW<PhysicsVelocity>>())
        {
            if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity player)) return;

            LocalTransform playerTransform = SystemAPI.GetComponent<LocalTransform>(player);

            float3 direction = math.normalize(playerTransform.Position - transform.ValueRO.Position);

            physics.ValueRW.Linear = direction * enemy.ValueRO.moveSpeed * SystemAPI.Time.DeltaTime;
            float3 currentPosition = transform.ValueRO.Position;

            currentPosition.x = math.clamp(currentPosition.x, -Config.Instance.GetPlaneSize(), Config.Instance.GetPlaneSize());
            currentPosition.y = 0;
            currentPosition.z = math.clamp(currentPosition.z, -Config.Instance.GetPlaneSize(), Config.Instance.GetPlaneSize());

            transform.ValueRW.Position = currentPosition;

            lastDirection = direction;
        }
    }
}

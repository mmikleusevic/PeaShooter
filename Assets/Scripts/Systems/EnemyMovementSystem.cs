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
        foreach (EnemyMovementAspect enemyMovement in SystemAPI.Query<EnemyMovementAspect>())
        {
            if (!SystemAPI.TryGetSingletonEntity<PlayerComponent>(out Entity player)) return;

            LocalTransform playerTransform = SystemAPI.GetComponent<LocalTransform>(player);

            float3 direction = math.normalize(playerTransform.Position - enemyMovement.transform.ValueRO.Position);
            direction.y = 0;

            enemyMovement.physics.ValueRW.Linear = direction * enemyMovement.enemyController.ValueRO.moveSpeed * SystemAPI.Time.DeltaTime;
        }
    }
}

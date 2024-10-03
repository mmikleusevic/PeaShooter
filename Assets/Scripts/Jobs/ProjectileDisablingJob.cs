using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
[WithNone(typeof(PlayerDeadComponent))]
public partial struct ProjectileDisablingJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;

    [ReadOnly] public ComponentLookup<EnemyComponent> enemyLookup;
    [ReadOnly] public float deltaTime;

    private void Execute([ChunkIndexInQuery] int sortKey, in Entity entity, ref ProjectileComponent projectile,
        ref LocalTransform transform, ref TargetComponent target, ref PhysicsVelocity velocity)
    {
        if (projectile.lifetime <= 0 || projectile.hasCollided == 1 || !enemyLookup.HasComponent(target.enemyEntity))
        {
            ecb.SetComponentEnabled<ProjectileComponent>(sortKey, entity, false);
            projectile.hasCollided = 0;
            projectile.lifetime = projectile.maxLifetime;
            target.enemy = default;
            target.enemyEntity = Entity.Null;
            velocity.Linear = 0;
            velocity.Angular = 0;

            //Don't want to destroy projectiles so I'll just move them out of sight
            transform.Position = new float3(-500, -500, -500);
        }

        projectile.lifetime -= deltaTime;
    }
}
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Jobs
{
    [BurstCompile]
    public partial struct ProjectileDisablingJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;

        [ReadOnly] public ComponentLookup<EnemyComponent> enemyComponentLookup;
        [ReadOnly] public float deltaTime;

        private void Execute([ChunkIndexInQuery] int sortKey, in Entity projectileEntity,
            ref ProjectileComponent projectileComponent,
            ref LocalTransform localTransform, ref TargetComponent targetComponent, ref PhysicsVelocity physicsVelocity)
        {
            if (projectileComponent.lifetime <= 0 || projectileComponent.hasCollided == 1 ||
                !enemyComponentLookup.HasComponent(targetComponent.enemyEntity))
            {
                ecb.SetComponentEnabled<ProjectileComponent>(sortKey, projectileEntity, false);
                projectileComponent.hasCollided = 0;
                projectileComponent.lifetime = projectileComponent.maxLifetime;
                targetComponent.enemyComponent = default;
                targetComponent.enemyEntity = Entity.Null;
                physicsVelocity.Linear = 0;
                physicsVelocity.Angular = 0;

                //Don't want to destroy projectiles so I'll just move them out of sight
                localTransform.Position = new float3(-500, -500, -500);
            }

            projectileComponent.lifetime -= deltaTime;
        }
    }
}
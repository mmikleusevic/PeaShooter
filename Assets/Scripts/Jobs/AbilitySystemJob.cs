using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct AbilitySystemJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;

    [ReadOnly] public Entity projectileEntity;
    [ReadOnly] public LocalTransform playerTransform;
    [ReadOnly] public float deltaTime;

    private void Execute([ChunkIndexInQuery] int sortKey, ref AbilityComponent ability)
    {
        if (ability.cooldownRemaining <= 0f)
        {
            if (projectileEntity == Entity.Null)
            {
                projectileEntity = ecb.Instantiate(sortKey, ability.projectileEntity);
            }
            else
            {
                ecb.SetComponentEnabled<ProjectileComponent>(sortKey, projectileEntity, true);
            }

            ecb.SetComponent(sortKey, projectileEntity, new LocalTransform
            {
                Position = playerTransform.Position + math.forward(playerTransform.Rotation)
            });

            ability.cooldownRemaining = ability.cooldown;
        }
        else
        {
            ability.cooldownRemaining -= deltaTime;
        }
    }
}
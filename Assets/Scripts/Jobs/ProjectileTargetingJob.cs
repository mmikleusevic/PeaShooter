#region

using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

#endregion

namespace Jobs
{
    [BurstCompile]
    [WithAll(typeof(ProjectileComponent))]
    public partial struct ProjectileTargetingJob : IJobEntity
    {
        [ReadOnly] public float deltaTime;
        [ReadOnly] public ComponentLookup<AbilityComponent> abilityComponentLookup;

        private void Execute(in TargetComponent target, ref LocalTransform localTransform,
            in ProjectileAbilityComponent projectileAbility, ref PhysicsVelocity physicsVelocity)
        {
            if (!abilityComponentLookup.HasComponent(projectileAbility.parentEntity)) return;

            RefRO<AbilityComponent> abilityComponent = abilityComponentLookup.GetRefRO(projectileAbility.parentEntity);

            float projectileSpeed = abilityComponent.ValueRO.speed * deltaTime;

            localTransform.Position =
                MoveTowards(localTransform.Position, target.enemyComponent.position, projectileSpeed);
        }

        [BurstCompile]
        private float3 MoveTowards(float3 current, float3 target, float maxDistanceDelta)
        {
            float num1 = target.x - current.x;
            float num2 = target.y - current.y;
            float num3 = target.z - current.z;
            float d = (float)(num1 * (double)num1 + num2 * (double)num2 + num3 * (double)num3);
            if (d == 0.0 || (maxDistanceDelta >= 0.0 && d <= maxDistanceDelta * (double)maxDistanceDelta))
                return target;
            float num4 = (float)math.sqrt((double)d);
            return new float3(current.x + num1 / num4 * maxDistanceDelta, current.y + num2 / num4 * maxDistanceDelta,
                current.z + num3 / num4 * maxDistanceDelta);
        }
    }
}
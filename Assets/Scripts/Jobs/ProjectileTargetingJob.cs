using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Jobs
{
    [BurstCompile]
    [WithAll(typeof(ProjectileComponent))]
    public partial struct ProjectileTargetingJob : IJobEntity
    {
        [ReadOnly] public float deltaTime;
        [ReadOnly] public ComponentLookup<AbilityComponent> abilityComponentLookup;

        private void Execute(ref TargetComponent targetComponent, ref LocalTransform localTransform,
            in ProjectileAbilityComponent projectileAbilityComponent)
        {
            if (!abilityComponentLookup.HasComponent(projectileAbilityComponent.parentEntity)) return;

            RefRO<AbilityComponent> abilityComponent =
                abilityComponentLookup.GetRefRO(projectileAbilityComponent.parentEntity);
            float3 moveDirection = math.normalize(targetComponent.enemyComponent.position - localTransform.Position);
            float distanceSqBefore = math.distancesq(localTransform.Position, targetComponent.enemyComponent.position);
            float projectileSpeed = abilityComponent.ValueRO.speed * deltaTime;

            localTransform.Position += moveDirection * projectileSpeed;

            float distanceSqAfter = math.distancesq(localTransform.Position, targetComponent.enemyComponent.position);

            if (distanceSqAfter < distanceSqBefore) return;

            localTransform.Position = targetComponent.enemyComponent.position;
        }
    }
}
using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct ProjectileTargetingJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    [ReadOnly] public ComponentLookup<AbilityComponent> AbilityLookup;

    private void Execute(in TargetComponent target, in ProjectileComponent projectile, in LocalTransform transform,
        ref PhysicsVelocity velocity, in ProjectileAbilityComponent projectileAbility)
    {
        AbilityComponent abilityComponent = AbilityLookup.GetRefRO(projectileAbility.parentEntity).ValueRO;

        float3 direction = math.normalize(target.enemy.position - transform.Position);

        velocity.Linear = direction * abilityComponent.speed * deltaTime;
    }
}
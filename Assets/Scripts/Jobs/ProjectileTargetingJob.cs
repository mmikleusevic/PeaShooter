using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
[WithAll(typeof(ProjectileComponent))]
public partial struct ProjectileTargetingJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    [ReadOnly] public ComponentLookup<AbilityComponent> AbilityLookup;

    private void Execute(in TargetComponent target, ref LocalTransform transform,
        in ProjectileAbilityComponent projectileAbility, ref PhysicsVelocity physicsVelocity)
    {
        if (!AbilityLookup.HasComponent(projectileAbility.parentEntity)) return;

        RefRO<AbilityComponent> abilityComponent = AbilityLookup.GetRefRO(projectileAbility.parentEntity);

        float3 direction = math.normalize(target.enemy.position - transform.Position);
        quaternion currentRotation = transform.Rotation;
        quaternion targetRotation = quaternion.LookRotation(direction, transform.Forward());
        float rotationSpeed = abilityComponent.ValueRO.rotationSpeed * deltaTime;

        transform.Rotation = math.slerp(currentRotation, targetRotation, rotationSpeed);
        physicsVelocity.Linear = transform.Forward() * abilityComponent.ValueRO.speed * deltaTime;
    }
}
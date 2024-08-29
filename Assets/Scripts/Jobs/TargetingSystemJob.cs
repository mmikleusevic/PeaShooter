using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct TargetingSystemJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<LocalTransform> localTransforms;
    [ReadOnly] public NativeArray<Entity> enemyEntities;

    private void Execute(ref TargetComponent target, in LocalTransform projectileTransform)
    {
        Entity closestEnemy = Entity.Null;
        float closestDistance = float.MaxValue;

        foreach (var enemyEntity in enemyEntities)
        {
            float3 enemyPosition = localTransforms[enemyEntity].Position;
            float distance = math.distance(projectileTransform.Position, enemyPosition);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemyEntity;
            }
        }

        target.targetEntity = closestEnemy;
    }
}

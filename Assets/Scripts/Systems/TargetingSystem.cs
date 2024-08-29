using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[BurstCompile]
public partial struct TargetingSystem : ISystem
{
    private EntityQuery enemyQuery;
    private ComponentLookup<LocalTransform> localTransforms;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        enemyQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<EnemyComponent>()
            .Build(ref state);

        localTransforms = state.GetComponentLookup<LocalTransform>(isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> enemyEntities = enemyQuery.ToEntityArray(Allocator.TempJob);
        localTransforms.Update(ref state);

        TargetingSystemJob job = new TargetingSystemJob
        {
            localTransforms = localTransforms,
            enemyEntities = enemyEntities
        };

        JobHandle jobHandle = job.ScheduleParallel(state.Dependency);
        state.Dependency = jobHandle;

        enemyEntities.Dispose();
    }
}

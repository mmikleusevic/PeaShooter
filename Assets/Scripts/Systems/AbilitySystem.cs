using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial struct AbilitySystem : ISystem
{
    private EntityQuery playerEntityQuery;
    private EntityQuery projectileEntityQuery;
    private EntityQuery enemyEntityQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        playerEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<PlayerComponent, LocalTransform>()
            .Build(ref state);

        projectileEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithDisabled<ProjectileComponent>()
            .Build(ref state);

        enemyEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<EnemyComponent>()
            .Build(ref state);

        state.RequireForUpdate(playerEntityQuery);
        state.RequireForUpdate(enemyEntityQuery);
        state.RequireForUpdate<AbilityComponent>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        LocalTransform playerTransform = playerEntityQuery.GetSingleton<LocalTransform>();

        BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        NativeArray<Entity> projectileEntities = projectileEntityQuery.ToEntityArray(Allocator.Temp);
        Entity projectileEntity = Entity.Null;

        if (projectileEntities.Length > 0)
        {
            projectileEntity = projectileEntities[0];
        }

        projectileEntities.Dispose();

        NativeArray<Entity> enemyEntities = enemyEntityQuery.ToEntityArray(Allocator.TempJob);

        AbilitySystemJob job = new AbilitySystemJob
        {
            ecb = ecb.AsParallelWriter(),
            enemyLookup = SystemAPI.GetComponentLookup<EnemyComponent>(true),
            enemyEntities = enemyEntities,
            projectileEntity = projectileEntity,
            playerTransform = playerTransform,
            deltaTime = SystemAPI.Time.DeltaTime,
        };

        JobHandle handle = job.ScheduleParallel(state.Dependency);

        state.Dependency = JobHandle.CombineDependencies(handle, enemyEntities.Dispose(handle));
    }
}
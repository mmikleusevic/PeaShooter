using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial struct AbilitySystem : ISystem
{
    private EntityQuery playerEntityQuery;
    private EntityQuery gridEntityQuery;
    private EntityQuery projectileEntityQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        playerEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<PlayerComponent, PlayerAliveComponent>()
            .Build(ref state);

        gridEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<GridComponent>()
            .Build(ref state);

        projectileEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithDisabled<ProjectileComponent>()
            .Build(ref state);

        state.RequireForUpdate(playerEntityQuery);
        state.RequireForUpdate(gridEntityQuery);
        state.RequireForUpdate<EnemyComponent>();
        state.RequireForUpdate<AbilityComponent>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
            SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        NativeArray<Entity> projectileEntities = projectileEntityQuery.ToEntityArray(Allocator.Temp);
        Entity projectileEntity = Entity.Null;

        if (projectileEntities.Length > 0) projectileEntity = projectileEntities[0];

        projectileEntities.Dispose();

        GridComponent gridComponent = gridEntityQuery.GetSingleton<GridComponent>();

        AbilityJob job = new AbilityJob
        {
            ecb = ecb.AsParallelWriter(),
            projectileEntity = projectileEntity,
            gridComponent = gridComponent,
            enemyLookup = SystemAPI.GetComponentLookup<EnemyComponent>(true),
            playerComponent = playerEntityQuery.GetSingleton<PlayerComponent>(),
            deltaTime = SystemAPI.Time.DeltaTime
        };

        JobHandle handle = job.ScheduleParallel(state.Dependency);
        state.Dependency = handle;
    }
}
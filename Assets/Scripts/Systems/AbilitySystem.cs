using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[BurstCompile]
public partial struct AbilitySystem : ISystem
{
    private BeginSimulationEntityCommandBufferSystem.Singleton ecbSingleton;
    private EntityQuery projectileEntityQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        projectileEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithDisabled<ProjectileComponent>()
            .Build(ref state);

        state.RequireForUpdate<PlayerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        LocalTransform playerTransform = SystemAPI.GetComponent<LocalTransform>(playerEntity);

        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        NativeArray<Entity> projectileEntities = projectileEntityQuery.ToEntityArray(Allocator.Temp);
        Entity projectileEntity = Entity.Null;

        if (projectileEntities.Length > 0)
        {
            projectileEntity = projectileEntities[0];
        }

        projectileEntities.Dispose();

        AbilitySystemJob job = new AbilitySystemJob
        {
            ecb = ecb.AsParallelWriter(),
            projectileEntity = projectileEntity,
            playerTransform = playerTransform,
            deltaTime = SystemAPI.Time.DeltaTime,
        };

        JobHandle handle = job.ScheduleParallel(state.Dependency);
        state.Dependency = handle;
    }
}
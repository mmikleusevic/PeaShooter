using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
[UpdateAfter(typeof(GridSpawnerSystem))]
[WithAll(typeof(PlayerSpawnerComponent))]

public partial struct PlayerSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerSpawnerComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        BeginInitializationEntityCommandBufferSystem.Singleton ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        PlayerSpawnJob job = new PlayerSpawnJob
        {
            ecb = ecb
        };

        JobHandle handle = job.Schedule(state.Dependency);
        state.Dependency = handle;
    }
}
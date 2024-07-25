using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
public partial struct ObstacleUpdateSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ObstacleComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        Entity entity = state.EntityManager.CreateEntity();

        DynamicBuffer<ObstacleBuffer> buffer = state.EntityManager.AddBuffer<ObstacleBuffer>(entity);

        ObstacleUpdateJob job = new ObstacleUpdateJob
        {
            buffer = buffer
        };

        job.Schedule();

        SystemHandle pathfindingSystemHandle = state.World.GetOrCreateSystem<PathfindingSystem>();

        state.WorldUnmanaged.ResolveSystemStateRef(pathfindingSystemHandle).Enabled = true;
    }
}
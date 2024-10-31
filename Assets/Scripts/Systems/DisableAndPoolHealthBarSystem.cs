using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
[UpdateAfter(typeof(UpdateHealthBarValueSystem))]
public partial struct DisableAndPoolHealthBarSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HealthBarUIReference>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    public void OnUpdate(ref SystemState state)
    {
        EndSimulationEntityCommandBufferSystem.Singleton ecbSingleton =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((HealthBarUIReference healthBarUI, Entity entity) in SystemAPI.Query<HealthBarUIReference>()
                     .WithNone<LocalTransform>()
                     .WithEntityAccess())
        {
            HealthBarPoolManager.Instance.ReturnHealthBar(healthBarUI.value);
            ecb.RemoveComponent<HealthBarUIReference>(entity);
        }
    }
}
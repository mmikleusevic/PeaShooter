using Unity.Entities;
using Unity.Physics;

public partial struct FreezeRotationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (freeRot, pmass) in SystemAPI.Query<
            RefRO<FreezeRotationComponent>, RefRW<PhysicsMass>>())
        {
            if (freeRot.ValueRO.flags.x) pmass.ValueRW.InverseInertia.x = 0f;
            if (freeRot.ValueRO.flags.y) pmass.ValueRW.InverseInertia.y = 0f;
            if (freeRot.ValueRO.flags.z) pmass.ValueRW.InverseInertia.z = 0f;
        }
    }
}

using Unity.Entities;
using Unity.Physics;

public partial struct FreezeRotationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (freezeRotation, physicalMass) in SystemAPI.Query<
            RefRO<FreezeRotationComponent>, RefRW<PhysicsMass>>())
        {
            if (freezeRotation.ValueRO.flags.x) physicalMass.ValueRW.InverseInertia.x = 0f;
            if (freezeRotation.ValueRO.flags.y) physicalMass.ValueRW.InverseInertia.y = 0f;
            if (freezeRotation.ValueRO.flags.z) physicalMass.ValueRW.InverseInertia.z = 0f;
        }
    }
}

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

[BurstCompile]
public partial struct FreezeRotationJob : IJobEntity
{
    [BurstCompile]
    public void Execute(ref PhysicsMass physicalMass, in FreezeRotationComponent freezeRotation)
    {
        float3 inverseInertia = physicalMass.InverseInertia;
        bool3 shouldFreeze = freezeRotation.flags & (inverseInertia != float3.zero);
        physicalMass.InverseInertia = math.select(inverseInertia, float3.zero, shouldFreeze);
    }
}
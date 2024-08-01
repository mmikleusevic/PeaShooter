using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

[BurstCompile]
public partial struct FreezeRotationJob : IJobEntity
{
    private void Execute(ref PhysicsMass physicalMass, in FreezeRotationComponent freezeRotation)
    {
        if (freezeRotation.flags.x) physicalMass.InverseInertia.x = 0f;
        if (freezeRotation.flags.y) physicalMass.InverseInertia.y = 0f;
        if (freezeRotation.flags.z) physicalMass.InverseInertia.z = 0f;
    }
}
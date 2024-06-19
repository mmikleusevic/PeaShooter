using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

public readonly partial struct PlayerMovementAspect : IAspect
{
    public readonly RefRW<PlayerComponent> playerController;
    public readonly RefRO<InputComponent> input;
    public readonly RefRW<LocalTransform> transform;
    public readonly RefRW<PhysicsVelocity> physics;
}

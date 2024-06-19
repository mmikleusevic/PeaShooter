using Unity.Entities;
using Unity.Transforms;

public readonly partial struct PlayerMovementAspect : IAspect
{
    public readonly RefRO<PlayerControllerComponent> playerController;
    public readonly RefRO<InputComponent> input;
    public readonly RefRW<LocalTransform> transform;
}

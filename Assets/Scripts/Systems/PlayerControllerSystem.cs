using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

[BurstCompile]
public partial class PlayerControllerSystem : SystemBase
{
    private PlayerInput playerInput;

    [BurstCompile]
    protected override void OnCreate()
    {
        playerInput = new PlayerInput();
        playerInput.Enable();
        playerInput.Player.Movement.performed += OnMovementPerformed;
        playerInput.Player.Movement.canceled += OnMovementCanceled;
    }

    [BurstCompile]
    protected override void OnUpdate()
    {

    }

    [BurstCompile]
    private void OnMovementCanceled(InputAction.CallbackContext obj)
    {
        SetMovement(Vector2.zero);
    }

    [BurstCompile]
    private void OnMovementPerformed(InputAction.CallbackContext obj)
    {
        SetMovement(playerInput.Player.Movement.ReadValue<Vector2>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        playerInput.Player.Movement.performed -= OnMovementPerformed;
        playerInput.Player.Movement.canceled -= OnMovementCanceled;
        playerInput.Disable();
    }

    [BurstCompile]
    private void SetMovement(Vector2 vector2)
    {
        if (!SystemAPI.TryGetSingletonEntity<InputComponent>(out Entity player)) return;

        InputComponent input = SystemAPI.GetComponent<InputComponent>(player);

        input.move = vector2;

        EntityManager.SetComponentData(player, input);
    }
}


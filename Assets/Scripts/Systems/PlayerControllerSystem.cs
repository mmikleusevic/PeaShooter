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

    private void OnMovementCanceled(InputAction.CallbackContext obj)
    {
        SetMovement(Vector2.zero);
    }

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

    private void SetMovement(Vector2 vector2)
    {
        foreach (RefRW<InputComponent> input in SystemAPI.Query<RefRW<InputComponent>>())
        {
            input.ValueRW.move = vector2;
        }
    }
}


using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial class PlayerControllerSystem : SystemBase
{
    private PlayerInput playerInput;

    protected override void OnCreate()
    {
        playerInput = new PlayerInput();

        playerInput.Enable();
        playerInput.Player.Movement.performed += OnMovementPerformed;
        playerInput.Player.Movement.canceled += OnMovementCanceled;

        RequireForUpdate<PlayerComponent>();
        RequireForUpdate<InputComponent>();
    }

    protected override void OnUpdate() { }

    private void OnMovementPerformed(InputAction.CallbackContext obj)
    {
        SetMovement(new float2(playerInput.Player.Movement.ReadValue<Vector2>()));
    }

    private void OnMovementCanceled(InputAction.CallbackContext obj)
    {
        SetMovement(float2.zero);
    }

    protected override void OnDestroy()
    {
        playerInput.Player.Movement.performed -= OnMovementPerformed;
        playerInput.Player.Movement.canceled -= OnMovementCanceled;
        playerInput.Disable();
    }

    private void SetMovement(float2 value)
    {
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();

        RefRW<InputComponent> input = SystemAPI.GetComponentRW<InputComponent>(playerEntity);

        input.ValueRW.move = value;
    }
}
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial class PlayerControllerSystem : SystemBase
{
    private PlayerInput playerInput;
    private EntityQuery inputEntityQuery;

    protected override void OnCreate()
    {
        playerInput = new PlayerInput();

        playerInput.Enable();
        playerInput.Player.Movement.performed += OnMovementPerformed;
        playerInput.Player.Movement.canceled += OnMovementCanceled;

        inputEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<InputComponent>()
            .Build(EntityManager);

        RequireForUpdate(inputEntityQuery);
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
        RefRW<InputComponent> input = inputEntityQuery.GetSingletonRW<InputComponent>();

        input.ValueRW.move = value;
    }
}
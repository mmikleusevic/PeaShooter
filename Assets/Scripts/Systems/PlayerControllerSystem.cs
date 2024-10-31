using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial class PlayerControllerSystem : SystemBase
{
    private EntityQuery inputEntityQuery;
    private PlayerInput playerInput;

    protected override void OnCreate()
    {
        base.OnCreate();

        playerInput = new PlayerInput();

        playerInput.Enable();
        playerInput.Player.Movement.performed += OnMovementPerformed;
        playerInput.Player.Movement.canceled += OnMovementCanceled;

        inputEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<InputComponent>()
            .Build(EntityManager);

        RequireForUpdate(inputEntityQuery);
        RequireForUpdate<PlayerAliveComponent>();
    }

    protected override void OnUpdate()
    {
    }

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
        base.OnDestroy();

        playerInput.Player.Movement.performed -= OnMovementPerformed;
        playerInput.Player.Movement.canceled -= OnMovementCanceled;
        playerInput.Disable();
    }

    private void SetMovement(float2 value)
    {
        if (inputEntityQuery.CalculateEntityCount() == 0) return;

        RefRW<InputComponent> input = inputEntityQuery.GetSingletonRW<InputComponent>();

        input.ValueRW.move = value;
    }
}
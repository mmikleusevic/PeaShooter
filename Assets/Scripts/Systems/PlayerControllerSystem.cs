using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial class PlayerControllerSystem : SystemBase
{
    private PlayerInput playerInput;
    private CollisionDamageSystem collisionDamageSystem;

    protected override void OnCreate()
    {
        playerInput = new PlayerInput();
        playerInput.Enable();
        playerInput.Player.Movement.performed += OnMovementPerformed;
        playerInput.Player.Movement.canceled += OnMovementCanceled;

        collisionDamageSystem = World.GetExistingSystemManaged<CollisionDamageSystem>();
        collisionDamageSystem.OnPlayerDied += CollisionDamageSystem_OnPlayerDied;
    }

    protected override void OnUpdate() { }

    private void OnMovementPerformed(InputAction.CallbackContext obj)
    {
        SetMovement(playerInput.Player.Movement.ReadValue<Vector2>());
    }

    private void OnMovementCanceled(InputAction.CallbackContext obj)
    {
        SetMovement(Vector2.zero);
    }

    private void CollisionDamageSystem_OnPlayerDied()
    {
        Enabled = false;
    }

    protected override void OnDestroy()
    {
        playerInput.Player.Movement.performed -= OnMovementPerformed;
        playerInput.Player.Movement.canceled -= OnMovementCanceled;
        collisionDamageSystem.OnPlayerDied -= CollisionDamageSystem_OnPlayerDied;
        playerInput.Disable();
    }

    private void SetMovement(Vector2 vector2)
    {
        Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();

        InputComponent input = SystemAPI.GetComponent<InputComponent>(playerEntity);

        input.move = vector2;

        EntityManager.SetComponentData(playerEntity, input);
    }
}
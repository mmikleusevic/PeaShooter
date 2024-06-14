using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;

    private PlayerInput playerInput;
    private Rigidbody playerRb;

    private Vector2 movementVector = Vector2.zero;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerRb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        playerInput.Enable();
        playerInput.Player.Movement.performed += OnMovementPerformed;
        playerInput.Player.Movement.canceled += OnMovementCanceled;
    }

    private void OnDisable()
    {
        playerInput.Disable();
        playerInput.Player.Movement.performed -= OnMovementPerformed;
        playerInput.Player.Movement.canceled -= OnMovementCanceled;
    }

    private void FixedUpdate()
    {
        playerRb.velocity = movementVector * moveSpeed * Time.deltaTime;
    }

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        movementVector = value.ReadValue<Vector2>();
    }

    private void OnMovementCanceled(InputAction.CallbackContext value)
    {
        movementVector = Vector2.zero;
    }
}

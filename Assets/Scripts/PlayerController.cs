using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float xBound = 21.5f;
    [SerializeField] private CameraBounds bounds;

    private PlayerInput playerInput;
    private Rigidbody playerRb;

    private Vector2 movementVector = Vector2.zero;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerRb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        float distanceFromCamera = Vector3.Distance(Camera.main.transform.position, transform.position);
        bounds.SetPerspectiveCameraBounds(distanceFromCamera);
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
        SetPlayerPosition();
        MovePlayer();
    }

    private void SetPlayerPosition()
    {
        Vector2 currentPosition = playerRb.position;

        currentPosition.x = Mathf.Clamp(currentPosition.x, -bounds.GetPerspectiveCameraBounds().x, bounds.GetPerspectiveCameraBounds().x);
        currentPosition.y = Mathf.Clamp(currentPosition.y, -bounds.GetPerspectiveCameraBounds().y, bounds.GetPerspectiveCameraBounds().y);

        playerRb.position = currentPosition;
    }

    private void MovePlayer()
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

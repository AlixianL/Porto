using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerControllerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("References")]
    [SerializeField] private PlayerInteractionDispatcher interactionDispatcher;

    [Header("Events")]
    public UnityEvent OnJumpPressed;

    [System.Serializable]
    public class MoveInputEvent : UnityEvent<Vector2> { }

    public MoveInputEvent OnMoveInput;

    private Rigidbody rb;

    [HideInInspector] public Vector2 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (interactionDispatcher == null)
            interactionDispatcher = GetComponent<PlayerInteractionDispatcher>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();
        OnMoveInput?.Invoke(direction);
    }

    private void Move()
    {
        if (rb == null)
            return;

        rb.linearVelocity = new Vector3(
            direction.x * movementSpeed,
            rb.linearVelocity.y,
            direction.y * movementSpeed
        );
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (rb != null)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        OnJumpPressed?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        Debug.Log("INPUT INTERACT reçu par : " + gameObject.name);

        if (interactionDispatcher == null)
        {
            Debug.LogError(gameObject.name + " : aucun PlayerInteractionDispatcher trouvé.");
            return;
        }

        interactionDispatcher.HandleInteract();
    }
}
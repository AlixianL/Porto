using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _jumpForce;

    private bool _canInteract;
    private bool _canControlled;
    private bool _canHandle;

    private bool _isWheel;
    private bool _isPedals;

    [SerializeField] private CarMovement otherController;

    public PlayerControllerMovement objectCurrentController;

    private Rigidbody _rb;
    public Vector2 direction;

    private ObjectThrower _objectThrower;
    private PlayerCarryThrower _playerCarryThrower;
    private TwoPlayerCarryInteractor _twoPlayerCarryInteractor;



    void Start()
    {
        
        _rb = GetComponent<Rigidbody>();

        _objectThrower = GetComponent<ObjectThrower>();
        _playerCarryThrower = GetComponent<PlayerCarryThrower>();
        _twoPlayerCarryInteractor = GetComponent<TwoPlayerCarryInteractor>();

        _canInteract = false;
        _canControlled = false;
        _canHandle = false;
    }

    public void FixedUpdate()
    {
        Move();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        direction = context.ReadValue<Vector2>();

        if (direction.sqrMagnitude > 0.1f)
            TutorialManager.Instance?.ValidateMove();
    }
    private void Move()
    {
        float Xmovement = direction.x * _movementSpeed * Time.fixedDeltaTime;
        float Ymovement = _rb.linearVelocity.y;
        float Zmovement = direction.y * _movementSpeed * Time.fixedDeltaTime;

        _rb.linearVelocity = new Vector3(Xmovement, Ymovement, Zmovement);
    }

    public void OnInteract()
    {
        if (_twoPlayerCarryInteractor != null)
        {
            bool handledHeavyObject = _twoPlayerCarryInteractor.TryHeavyCarryAction();

            if (handledHeavyObject)
                return;
        }

        if (_objectThrower != null)
            _objectThrower.TryObjectAction();

        if (_playerCarryThrower != null)
            _playerCarryThrower.TryCarryAction();

        if (_canInteract)
            print("interact");

        if (_canHandle)
            print("can lift other player");
    }

    public void OnJump()
    {
        _rb.AddForce(new Vector3(0f, _jumpForce, 0f), ForceMode.Impulse);
    }

    public void OnSwitchController()
    {
        if (_canControlled && otherController != null)
        {
            print("Switch");
           

            direction = Vector2.zero;
            _rb.linearVelocity = Vector3.zero;

            PlayerInput playerInput = GetComponent<PlayerInput>();
            InputDevice device = playerInput.devices[0];

            PlayerInput doorInput = null;

            if (_isWheel)
                doorInput = otherController.GetComponentsInChildren<PlayerInput>()[0];
            else
                doorInput = otherController.GetComponentsInChildren<PlayerInput>()[1];

            if (doorInput != null)
                doorInput.SwitchCurrentControlScheme(playerInput.currentControlScheme, device);

            otherController.PlayerEnter(_isWheel);

            _canControlled = false;
            playerInput.enabled = false;
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftDoor"))
        {
            CarMovement car = other.GetComponentInParent<CarMovement>();

            if (car != null)
            {
                otherController = car;
                _canControlled = true;
                _isWheel = true;
                _isPedals = false;
            }
        }
        else if (other.CompareTag("RightDoor"))
        {
            CarMovement car = other.GetComponentInParent<CarMovement>();

            if (car != null)
            {
                otherController = car;
                _canControlled = true;
                _isPedals = true;
                _isWheel = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LeftDoor") || other.CompareTag("RightDoor"))
        {
            if (otherController != null)
                otherController.PlayerExit(_isWheel);

            otherController = null;
            _canControlled = false;
            _isWheel = false;
            _isPedals = false;
        }
    }
}
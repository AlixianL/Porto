using UnityEngine;
using UnityEngine.InputSystem;

public class CarMovement : MonoBehaviour
{
    private Rigidbody _rb;

    [Header("Movement Stats")]
    [SerializeField] private float _acceleration;
    private float _actualAcceleration;
    private float _actualSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _friction;
    [SerializeField] private float _breakForce;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _rotationSpeed;

    [Header("State")]
    public bool forwardInput;
    public bool backwardInput;
    public float axis;
    public bool jumpPedal;
    public bool jumpWheel;
    public bool isInLevel;

    [SerializeField] private bool inputLocked;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private float _distanceRaycast;
    [SerializeField] private Vector3 _offsetRaycast;

    private float _direction;

    public bool _wheelController;
    public bool _pedalsController;

    private int _playersInCar = 0;

    public CameraManager cameraManager;
    public AudioSource audioSource;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.centerOfMass = new Vector3(0f, -0.5f, 0f);

        if (_rb != null)
            _rb.maxLinearVelocity = _maxSpeed;

        if (cameraManager == null)
            cameraManager = FindAnyObjectByType<CameraManager>();

        UpdatePlayersInCarCount();
        UpdateCameraState();
    }

    private void Update()
    {
        if (_rb == null)
            return;

        if ((_rb.linearVelocity.y <= 0.001f && _rb.linearVelocity.z <= 0.001f) &&
            (_rb.linearVelocity.y >= -0.001f && _rb.linearVelocity.z >= -0.001f))
        {
            _direction = 0f;

            if (forwardInput && !backwardInput)
                _direction = 1f;
            else if (backwardInput && !forwardInput)
                _direction = -1f;
        }
    }

    private void FixedUpdate()
    {
        if (_rb == null)
            return;

        Debug.DrawLine(
            transform.position + _offsetRaycast,
            transform.position - new Vector3(0f, _distanceRaycast, 0f),
            Color.red
        );

        _isGrounded = Physics.Raycast(
            transform.position + _offsetRaycast,
            Vector3.down,
            _distanceRaycast,
            LayerMask.GetMask("Ground", "Landingground")
        );

        if (inputLocked)
        {
            OnDecelerate(1);
            return;
        }

        if (_isGrounded && _wheelController && _pedalsController)
        {
            if (_direction > 0f)
            {
                if (backwardInput)
                    OnBreak(1);
                else if (forwardInput)
                    OnAcceleration(1);
                else
                    OnDecelerate(1);

                OnTurn(axis);
            }
            else if (_direction < 0f)
            {
                if (forwardInput)
                    OnBreak(-1);
                else if (backwardInput)
                    OnAcceleration(-1);
                else
                    OnDecelerate(-1);

                OnTurn(axis * -1);
            }
            else
            {
                OnDecelerate(1);
            }

            if (jumpPedal && jumpWheel)
                OnJump();
        }
    }

    public void PlayerEnter(bool isWheel)
    {
        if (isWheel)
            _wheelController = true;
        else
            _pedalsController = true;

        UpdatePlayersInCarCount();
        UpdateCameraState();
    }

    public void PlayerExit(bool isWheel)
    {
        if (isWheel)
            _wheelController = false;
        else
            _pedalsController = false;

        UpdatePlayersInCarCount();
        UpdateCameraState();
    }

    private void UpdatePlayersInCarCount()
    {
        _playersInCar = 0;

        if (_wheelController)
            _playersInCar++;

        if (_pedalsController)
            _playersInCar++;
    }

    private void UpdateCameraState()
    {
        if (cameraManager == null)
            return;

        bool shouldUseCarCamera = _playersInCar >= 2;

        cameraManager.SetCarCamera(shouldUseCarCamera);
    }

    public void SetInputLocked(bool locked)
    {
        inputLocked = locked;

        if (locked)
            ResetCarMovement();
    }

    public void StopCarImmediately()
    {
        ResetCarMovement();

        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }

    public void OnAccelerator(InputAction.CallbackContext context)
    {
        if (inputLocked)
            return;

        forwardInput = context.ReadValue<float>() > 0.1f;
    }

    public void OnBreakInput(InputAction.CallbackContext context)
    {
        if (inputLocked)
            return;

        backwardInput = context.ReadValue<float>() > 0.1f;
    }

    public void OnJumpPedal(InputAction.CallbackContext context)
    {
        if (inputLocked)
            return;

        jumpPedal = context.performed;
    }

    public void OnSteer(InputAction.CallbackContext context)
    {
        if (inputLocked)
            return;

        axis = context.ReadValue<float>();
    }

    public void OnJumpWheel(InputAction.CallbackContext context)
    {
        if (inputLocked)
            return;

        jumpWheel = context.performed;
    }

    public void OnAcceleration(float direction)
    {
        float result = _actualAcceleration + _acceleration * Time.fixedDeltaTime;
        _actualAcceleration = Mathf.Clamp(result, 0f, _maxSpeed);

        result = _actualSpeed + _actualAcceleration * Time.fixedDeltaTime;
        _actualSpeed = Mathf.Clamp(result, 0f, _maxSpeed);

        float verticalVelocity = _rb.linearVelocity.y;

        _rb.linearVelocity = transform.forward * _actualSpeed * direction;
        _rb.linearVelocity = new Vector3(
            _rb.linearVelocity.x,
            verticalVelocity,
            _rb.linearVelocity.z
        );
    }

    public void OnDecelerate(float direction)
    {
        float result = _actualAcceleration - _acceleration * Time.fixedDeltaTime;
        _actualAcceleration = Mathf.Clamp(result, 0f, _maxSpeed);

        result = _actualSpeed - _friction * Time.fixedDeltaTime;
        _actualSpeed = Mathf.Clamp(result, 0f, _maxSpeed);

        float verticalVelocity = _rb.linearVelocity.y;

        _rb.linearVelocity = transform.forward * _actualSpeed * direction;
        _rb.linearVelocity = new Vector3(
            _rb.linearVelocity.x,
            verticalVelocity,
            _rb.linearVelocity.z
        );
    }

    public void OnBreak(float direction)
    {
        float result = _actualAcceleration - _breakForce * Time.fixedDeltaTime;
        _actualAcceleration = Mathf.Clamp(result, 0f, _maxSpeed);

        result = _actualSpeed - _breakForce * Time.fixedDeltaTime;
        _actualSpeed = Mathf.Clamp(result, 0f, _maxSpeed);

        float verticalVelocity = _rb.linearVelocity.y;

        _rb.linearVelocity = transform.forward * _actualSpeed * direction;
        _rb.linearVelocity = new Vector3(
            _rb.linearVelocity.x,
            verticalVelocity,
            _rb.linearVelocity.z
        );
    }

    public void OnTurn(float inputAxis)
    {
        if (_rb == null)
            return;

        float turnAmount =
            inputAxis * _rotationSpeed * (_actualSpeed / 10f) * Time.fixedDeltaTime;

        Quaternion deltaRotation = Quaternion.Euler(0f, turnAmount, 0f);
        Quaternion targetRotation = _rb.rotation * deltaRotation;

        _rb.MoveRotation(targetRotation);
    }

    private void OnJump()
    {
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Level"))
        {
            isInLevel = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Level"))
        {
            isInLevel = false;
        }
    }

    public void OnKlaxon()
    {
        if (audioSource != null)
            audioSource.Play();
    }

    public void ResetCarMovement()
    {
        forwardInput = false;
        backwardInput = false;
        axis = 0f;
        jumpPedal = false;
        jumpWheel = false;

        _actualAcceleration = 0f;
        _actualSpeed = 0f;
        _direction = 0f;
    }
}
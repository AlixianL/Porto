using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarMovement : MonoBehaviour
{
    private Rigidbody _rb;

    [Header("Movement Stats :")]
    [SerializeField] private float _acceleration;
    private float _actualAcceleration;
    private float _actualSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _friction;
    [SerializeField] private float _breakForce;
    [SerializeField] private float _jumpForce;

    [SerializeField] private float _rotationSpeed;

    [Header("Debug booléans : ")]
    public bool forwardInput;
    public bool backwardInput;
    public float axis;
    public bool jumpPedal;
    public bool jumpWheel;
    public bool isInLevel;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private float _distanceRaycast;
    [SerializeField] private Vector3 _offsetRaycast;
    private float _direction;

    public bool _wheelController;
    public bool _pedalsController;
    private int _playersInCar = 0;
    public CameraManager cameraManager;

    public AudioSource audioSource;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.maxLinearVelocity = _maxSpeed;
        cameraManager = GetComponentInChildren<CameraManager>();
    }

    private void Update()
    {
        if ((_rb.linearVelocity.y <= 0.001f && _rb.linearVelocity.z <= 0.001f) &&
            (_rb.linearVelocity.y >= -0.001f && _rb.linearVelocity.z >= -0.001f))
        {
            _direction = 0f;
            if (forwardInput && !backwardInput) _direction = 1f;
            else if (backwardInput && !forwardInput) _direction = -1f;
        }
    }

    void FixedUpdate()
    {
        Debug.DrawLine(gameObject.transform.position + _offsetRaycast,
            gameObject.transform.position - new Vector3(0f, _distanceRaycast, 0f), Color.red);
        _isGrounded = Physics.Raycast(gameObject.transform.position + _offsetRaycast,
            Vector2.down, _distanceRaycast, LayerMask.GetMask("Ground"));

        if (_isGrounded && _wheelController && _pedalsController)
        {
            if (_direction > 0f)
            {
                if (backwardInput) OnBreak(1);
                else if (forwardInput) OnAcceleration(1);
                else OnDecelerate(1);
                OnTurn(axis);
            }
            else if (_direction < 0f)
            {
                if (forwardInput) OnBreak(-1);
                else if (backwardInput) OnAcceleration(-1);
                else OnDecelerate(-1);
                OnTurn(axis * -1);
            }
            else
            {
                OnDecelerate(1);
            }

            if (jumpPedal && jumpWheel) OnJump();
        }
    }

    public void PlayerEnter(bool isWheel)
    {
        if (isWheel)
            _wheelController = true;
        else
            _pedalsController = true;

        _playersInCar++;

        if (_playersInCar >= 2)
            cameraManager.ChangeCamera();
    }

    public void PlayerExit(bool isWheel)
    {
        if (isWheel)
            _wheelController = false;
        else
            _pedalsController = false;

        if (_playersInCar >= 2)
            cameraManager.ChangeCamera();

        _playersInCar--;
        _playersInCar = Mathf.Max(0, _playersInCar);
    }

    public void OnAccelerator(InputAction.CallbackContext context)
    {
        forwardInput = context.ReadValue<float>() > 0.1f;
    }

    public void OnBreakInput(InputAction.CallbackContext context)
    {
        backwardInput = context.ReadValue<float>() > 0.1f;
    }

    public void OnJumpPedal(InputAction.CallbackContext context)
    {
        jumpPedal = context.performed;
    }

    public void OnSteer(InputAction.CallbackContext context)
    {
        axis = context.ReadValue<float>();
    }

    public void OnJumpWheel(InputAction.CallbackContext context)
    {
        jumpWheel = context.performed;
    }

    public void OnAcceleration(float direction)
    {
        float result = _actualAcceleration + _acceleration * Time.fixedDeltaTime;
        _actualAcceleration = Mathf.Clamp(result, 0f, _maxSpeed);
        result = _actualSpeed + _actualAcceleration * Time.fixedDeltaTime;
        _actualSpeed = Mathf.Clamp(result, 0f, _maxSpeed);
        float tmp = _rb.linearVelocity.y;
        _rb.linearVelocity = gameObject.transform.forward * _actualSpeed * direction;
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, tmp, _rb.linearVelocity.z);
    }

    public void OnDecelerate(float direction)
    {
        float result = _actualAcceleration - _acceleration * Time.fixedDeltaTime;
        _actualAcceleration = Mathf.Clamp(result, 0f, _maxSpeed);
        result = _actualSpeed - _friction * Time.fixedDeltaTime;
        _actualSpeed = Mathf.Clamp(result, 0f, _maxSpeed);
        float tmp = _rb.linearVelocity.y;
        _rb.linearVelocity = gameObject.transform.forward * _actualSpeed * direction;
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, tmp, _rb.linearVelocity.z);
    }

    public void OnBreak(float direction)
    {
        float result = _actualAcceleration - _breakForce * Time.fixedDeltaTime;
        _actualAcceleration = Mathf.Clamp(result, 0f, _maxSpeed);
        result = _actualSpeed - _breakForce * Time.fixedDeltaTime;
        _actualSpeed = Mathf.Clamp(result, 0f, _maxSpeed);
        float tmp = _rb.linearVelocity.y;
        _rb.linearVelocity = gameObject.transform.forward * _actualSpeed * direction;
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, tmp, _rb.linearVelocity.z);
    }

    public void OnTurn(float axis)
    {
        float Yrotation = gameObject.transform.eulerAngles.y + axis * _rotationSpeed * (_actualSpeed / 10) * Time.fixedDeltaTime;
        gameObject.transform.eulerAngles = new Vector3(0f, Yrotation, 0f);
    }

    private void OnJump()
    {
        _rb.AddForce(new Vector3(0f, _jumpForce, 0f), ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Level"))
        {
            isInLevel = true;
            cameraManager._levelCamera = other.GetComponentInChildren<Camera>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Level")) isInLevel = false;
    }

    public void OnKlaxon()
    {
        print("fahhhhhhhh");
        audioSource.Play();
    }

   
   

    

}
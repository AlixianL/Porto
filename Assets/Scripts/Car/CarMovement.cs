using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    private Rigidbody _rb;

    #region acceleration
    [Header("Movement Stats :")]
    [SerializeField] private float _acceleration;
    private float _actualAcceleration;
    private float _actualSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _friction;
    [SerializeField] private float _breakForce;
    [SerializeField] private float _jumpForce;
    #endregion



    #region Rotation
    [SerializeField] private float _rotationSpeed;
    #endregion

    #region VariblesTest
    [Header("Debug booléans : ")]
    public bool forwardInput;
    public bool backwardInput;
    public float axis;
    public bool jumpPedal;
    public bool jumpWheel;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private float _distanceRaycast;
    [SerializeField] private Vector3 _offsetRaycast;
    private float _direction;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.maxLinearVelocity = _maxSpeed;
    }

    private void Update()
    {
        /*
        // Pedals => forward / backWard / jumpPedal
        // Wheel => axis / jumpWheel
        forwardInput = Input.GetKey(KeyCode.W);
        backwardInput = Input.GetKey(KeyCode.S);
        //_canReverse = (_rb.linearVelocity.y <= 0f && _rb.linearVelocity.z <= 0f )? true : false;
        axis = Input.GetKey(KeyCode.D) ? 1f : Input.GetKey(KeyCode.A) ? -1f : 0f;

        jumpPedal = Input.GetKey(KeyCode.Space);
        jumpWheel = Input.GetKey(KeyCode.Space);
        */

        if ((_rb.linearVelocity.y <= 0.001f && _rb.linearVelocity.z <= 0.001f) && (_rb.linearVelocity.y >= -0.001f && _rb.linearVelocity.z >= -0.001f))
        {
            _direction = 0f;
            if (forwardInput && !backwardInput)
            {
                _direction = 1f;
            }
            else if (backwardInput && !forwardInput)
            {
                _direction = -1f;
            }
            print(_direction);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.DrawLine(gameObject.transform.position + _offsetRaycast, gameObject.transform.position - new Vector3(0f,_distanceRaycast,0f), Color.red);
        _isGrounded = Physics.Raycast(gameObject.transform.position + _offsetRaycast, Vector2.down, _distanceRaycast, LayerMask.GetMask("Ground"));
        if (_isGrounded)
        {
            if (_direction > 0f)
            {
                if (backwardInput)
                {
                    Break(1);
                }
                else if (forwardInput)
                {
                    Acceleration(1);
                }
                else
                {
                    Decelerate(1);
                }
                Turn(axis);
            }

            else if (_direction < 0f)
            {
                if (forwardInput)
                {
                    Break(-1);
                }
                else if (backwardInput)
                {
                    Acceleration(-1);
                }
                else
                {
                    Decelerate(-1);
                }
                Turn(axis * -1);
            }

            else
            {
                Decelerate(1);
            }
                

            if (jumpPedal && jumpWheel)
            {
                Jump();
            }
        }
    }

    public void Acceleration(float direction)
    {
        float result = _actualAcceleration + _acceleration * Time.fixedDeltaTime;
        _actualAcceleration = Mathf.Clamp(result, 0f, _maxSpeed);
        
        result = _actualSpeed + _actualAcceleration * Time.fixedDeltaTime;
        _actualSpeed = Mathf.Clamp(result,0f,_maxSpeed);
        float tmp = _rb.linearVelocity.y; 
        _rb.linearVelocity = gameObject.transform.forward * _actualSpeed * direction;
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, tmp, _rb.linearVelocity.z);
    }

    public void Decelerate(float direction)
    {
        float result = _actualAcceleration - _acceleration * Time.fixedDeltaTime;
        _actualAcceleration = Mathf.Clamp(result, 0f, _maxSpeed);

        result = _actualSpeed - _friction * Time.fixedDeltaTime;
        _actualSpeed = Mathf.Clamp(result, 0f, _maxSpeed);
        float tmp = _rb.linearVelocity.y;
        _rb.linearVelocity = gameObject.transform.forward * _actualSpeed * direction;
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, tmp, _rb.linearVelocity.z);
    }

    public void Break(float direction)
    {
        float result = _actualAcceleration - _breakForce * Time.fixedDeltaTime;
        _actualAcceleration = Mathf.Clamp(result, 0f, _maxSpeed);

        result = _actualSpeed - _breakForce * Time.fixedDeltaTime;
        _actualSpeed = Mathf.Clamp(result, 0f, _maxSpeed);
        float tmp = _rb.linearVelocity.y;
        _rb.linearVelocity = gameObject.transform.forward * _actualSpeed * direction;
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, tmp, _rb.linearVelocity.z);
    }

    public void Turn(float axis)
    {
        float Yrotation = gameObject.transform.eulerAngles.y + axis * _rotationSpeed * (_actualSpeed / 10) * Time.fixedDeltaTime;
        gameObject.transform.eulerAngles = new Vector3(0f, Yrotation ,0f);
    }

    private void Jump()
    {
        _rb.AddForce(new Vector3(0f,_jumpForce,0f), ForceMode.Impulse);
    }
}
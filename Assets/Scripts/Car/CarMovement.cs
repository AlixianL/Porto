using System;
using UnityEngine;

public class CaMovement : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private float _acceleration;
    private float _actualAcceleration;
    private float _actualSpeed;
    [SerializeField] private float _maxSpeed;
    private bool _canAccelerate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.maxLinearVelocity = _maxSpeed;
    }

    private void Update()
    {
        _canAccelerate = Input.GetKey(KeyCode.Space);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_canAccelerate)
        {
            Acceleration();
        }
    }

    protected void Acceleration()
    {
        float result = _actualAcceleration + _acceleration * Time.fixedDeltaTime;
        _actualAcceleration = Mathf.Clamp(result, 0f, _maxSpeed);
        
        result = _actualSpeed + _actualAcceleration * Time.fixedDeltaTime;
        _actualSpeed = Mathf.Clamp(result,0f,_maxSpeed);


        float Xmovement = _rb.linearVelocity.x;
        float Ymovement = _rb.linearVelocity.y;
        float Zmovement = _actualSpeed;

        _rb.linearVelocity = new Vector3(Xmovement, Ymovement, Zmovement);
    }
}

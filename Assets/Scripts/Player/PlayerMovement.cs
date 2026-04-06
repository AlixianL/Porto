using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _jumpForce;
    private bool _canInteract;
    private bool _canControlled;
    private Controller otherController;
    public Controller objectCurrentController;
    private Rigidbody _rb;
    public Vector2 direction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _canInteract = false;
        _canControlled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Move();
    }

    protected void Move()
    {
        float Xmovement = direction.x * _movementSpeed * Time.fixedDeltaTime;
        float Ymovement = _rb.linearVelocity.y;
        float Zmovement = direction.y * _movementSpeed * Time.fixedDeltaTime;
        _rb.linearVelocity = new Vector3(Xmovement, Ymovement , Zmovement);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactible"))
        {
            _canInteract = true;
        }
        else if (other.CompareTag("Controllable"))
        {
            _canControlled = true;
            otherController = other.gameObject.GetComponent<Controller>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Interactible"))
        {
            _canInteract = false;
        }
        else if (other.CompareTag("Controllable"))
        {
            _canControlled = false;
            otherController = null;
        }
    }

    public void Interact()
    {
        if (_canInteract)
        {
            print("interact");
        }
    }

    public void Jump()
    {
        _rb.AddForce(new Vector3(0f,_jumpForce,0f), ForceMode.Impulse);
    }

    public void TakeControl()
    {
        if (_canControlled)
        {
            otherController.enabled = true;
            otherController.indexGamepad = objectCurrentController.indexGamepad;
            otherController.otherController = objectCurrentController;
            this.enabled = false;
        }
    }
}

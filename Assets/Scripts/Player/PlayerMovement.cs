using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _jumpForce;

    private bool _canInteract;
    private bool _canControlled;
    private bool _canHandle;

    private Controller otherController;
    public Controller objectCurrentController;

    private Rigidbody _rb;
    public Vector2 direction;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _canInteract = false;
        _canControlled = false;
        _canHandle = false;
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

        _rb.linearVelocity = new Vector3(Xmovement, Ymovement, Zmovement);
    }

    public void Interact()
    {
        if (_canInteract)
        {
            print("interact");
        }

        if (_canHandle)
        {
            print("can lift other player");
        }
    }

    public void Jump()
    {
        _rb.AddForce(new Vector3(0f, _jumpForce, 0f), ForceMode.Impulse);
    }

    public void TakeControl()
    {
        if (_canControlled)
        {
            if (objectCurrentController.isKeyboard)
            {
                otherController.isKeyboard = true;
            }
            else
            {
                otherController.indexGamepad = objectCurrentController.indexGamepad;
                otherController.isKeyboard = false;
            }

            otherController.controllerToSwitch = objectCurrentController;

            gameObject.transform.SetParent(otherController.gameObject.transform, true);
            otherController.enabled = true;
            gameObject.SetActive(false);
        }
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

            otherController = other.GetComponent<Controller>();

            if (otherController == null)
                otherController = other.GetComponentInParent<Controller>();

            print("rentre en collision");
        }
        else if (other.CompareTag("Player"))
        {
            _canHandle = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactible"))
        {
            _canInteract = false;
        }
        else if (other.CompareTag("Controllable"))
        {
            _canControlled = false;
            otherController = null;
            print("sort de la collision");
        }
        else if (other.CompareTag("Player"))
        {
            _canHandle = false;
        }
    }
}
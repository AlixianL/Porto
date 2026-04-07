using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerController : Controller
{
    [Header("Child Particularities :")]
    [SerializeField] private PlayerMovement _playerMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerMovement.objectCurrentController = this;
        print("child test");
    }

    public override void Update()
    {
        base.Update();
        if (!isKeyboard)
        {
            if (gamepad.buttonSouth.wasPressedThisFrame)
            {
                _playerMovement.Jump();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _playerMovement.Jump();
            }
        }
    }

    public override void SwitchController()
    {
        _playerMovement.TakeControl();
    }

    public override void Horizontal(Vector2 direction)
    {
        _playerMovement.direction = direction;
    }

    public override void InteractInput()
    {
        _playerMovement.Interact();
    }

    public override void PositiveForce()
    {
        
    }

    public override void NegativeForce()
    {
        
    }
}
      
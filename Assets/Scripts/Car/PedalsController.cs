using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PedalsController : Controller
{
    [SerializeField] private CarMovement _carMovement;
    [SerializeField] private float _delayJumpInput;
    private BoxCollider _collider;

    public override void Start()
    {
    }

    public void OnEnable()
    {
        if (!isKeyboard)
        {
            gamepad = Gamepad.all[indexGamepad];
        }
        _collider = GetComponent<BoxCollider>();

        _carMovement._pedalsController = true;
        _collider.enabled = false;

        if (_carMovement._wheelController)
        {
            _carMovement.cameraManager.ChangeCamera();
        }
        print("Pedals enabled");
    }

    // Update is called once per frame
    public override void Update()
    {
        _carMovement.forwardInput = false;
        _carMovement.backwardInput = false;
        base.Update();

        if (!isKeyboard)
        {
            if (gamepad.buttonSouth.wasPressedThisFrame)
            {
                StartCoroutine(JumpTimer());
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(JumpTimer());
            }
        }
    }

    public override void SwitchController()
    {
        if ( _carMovement.isInLevel)
        {
            _collider.enabled = true;
            _carMovement._pedalsController = false;
            if (_carMovement._wheelController)
            {
                _carMovement.cameraManager.ChangeCamera();
            }
            transform.GetChild(0).gameObject.SetActive(true);
            base.SwitchController();
        }
    }

    public override void PositiveForce()
    {
        _carMovement.forwardInput = true;
    }

    public override void NegativeForce()
    {
        _carMovement.backwardInput = true;
    }

    IEnumerator JumpTimer()
    {
        _carMovement.jumpPedal = true;
        yield return new WaitForSeconds(_delayJumpInput);
        _carMovement.jumpPedal = false;
    }
}

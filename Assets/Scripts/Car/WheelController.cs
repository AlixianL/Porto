using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WheelController : Controller
{
    [SerializeField] private CarMovement _carMovement;
    [SerializeField] private float _delayJumpInput;
    private BoxCollider _collider;

 

    public void OnEnable()
    {

        _carMovement._wheelController = true;
        _collider.enabled = false;
        if (_carMovement._pedalsController)
        {
            _carMovement.cameraManager.ChangeCamera();
        }
        print("Wheel enabled");
    }

    // Update is called once per frame
    public override void Update()
    {
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
        if (_carMovement.isInLevel)
        {
            _collider.enabled = true;
            _carMovement._wheelController = false;
            if (_carMovement._pedalsController)
            {
                _carMovement.cameraManager.ChangeCamera();
            }
            transform.GetChild(0).gameObject.SetActive(true);
            base.SwitchController();
        }
    }

    public override void Horizontal(Vector2 direction)
    {
        _carMovement.axis = direction.x;
    }

    IEnumerator JumpTimer()
    {
        _carMovement.jumpWheel = true;
        yield return new WaitForSeconds(_delayJumpInput);
        _carMovement.jumpWheel = false;
    }
}

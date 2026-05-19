using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class WheelController : Controller
{
    [Header("Car")]
    [SerializeField] private CarMovement _carMovement;
    [SerializeField] private float _delayJumpInput = 0.2f;

    [Header("Events")]
    public UnityEvent OnWheelInput;

    private void Awake()
    {
        if (_carMovement == null)
            _carMovement = GetComponentInParent<CarMovement>();
    }

    public override void Update()
    {
        base.Update();

        if (!isKeyboard)
        {
            if (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame)
                StartCoroutine(JumpTimer());
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
                StartCoroutine(JumpTimer());
        }
    }

    public override void Horizontal(Vector2 direction)
    {
        if (_carMovement != null)
            _carMovement.axis = direction.x;

        if (Mathf.Abs(direction.x) > 0.1f)
            OnWheelInput?.Invoke();
    }

    private IEnumerator JumpTimer()
    {
        if (_carMovement == null)
            yield break;

        _carMovement.jumpWheel = true;

        yield return new WaitForSeconds(_delayJumpInput);

        if (_carMovement != null)
            _carMovement.jumpWheel = false;
    }
}
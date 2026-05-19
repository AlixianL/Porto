using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PedalsController : Controller
{
    [Header("Car")]
    [SerializeField] private CarMovement _carMovement;
    [SerializeField] private float _delayJumpInput = 0.2f;

    [Header("Events")]
    public UnityEvent OnPedalsInput;

    private void Awake()
    {
        if (_carMovement == null)
            _carMovement = GetComponentInParent<CarMovement>();
    }

    public override void Update()
    {
        if (_carMovement == null)
            return;

        _carMovement.forwardInput = false;
        _carMovement.backwardInput = false;

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

    public override void PositiveForce()
    {
        if (_carMovement != null)
            _carMovement.forwardInput = true;

        OnPedalsInput?.Invoke();
    }

    public override void NegativeForce()
    {
        if (_carMovement != null)
            _carMovement.backwardInput = true;

        OnPedalsInput?.Invoke();
    }

    private IEnumerator JumpTimer()
    {
        if (_carMovement == null)
            yield break;

        _carMovement.jumpPedal = true;

        yield return new WaitForSeconds(_delayJumpInput);

        if (_carMovement != null)
            _carMovement.jumpPedal = false;
    }
}
using UnityEngine;
using System.Collections;


public class PedalsController : Controller
{
    [SerializeField] private CarMovement _carMovement;
    [SerializeField] private float _delayJumpInput;

    public override void Start()
    {
        base.Start();
        otherController = this;
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

    public override void Horizontal(Vector2 direction)
    {
        _carMovement.axis = direction.x;
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

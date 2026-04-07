using System.Collections;
using UnityEngine;

public class WheelController : Controller
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

    IEnumerator JumpTimer()
    {
        _carMovement.jumpWheel = true;
        yield return new WaitForSeconds(_delayJumpInput);
        _carMovement.jumpWheel = false;
    }
}

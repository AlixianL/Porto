using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PedalsController : Controller
{
    [SerializeField] private CarMovement _carMovement;
    [SerializeField] private float _delayJumpInput;
    [SerializeField] private KeyCode exitVehicleKey = KeyCode.E;

    private BoxCollider _collider;

    public override void Start()
    {
        base.Start();
        _collider = GetComponent<BoxCollider>();
    }

    private void OnEnable()
    {
        RefreshInputDevice();

        if (_collider == null)
            _collider = GetComponent<BoxCollider>();

        if (_carMovement == null)
            _carMovement = GetComponentInParent<CarMovement>();

        if (_carMovement == null)
        {
            Debug.LogWarning(gameObject.name + " : aucun CarMovement assigné au PedalsController.");
            return;
        }

        _carMovement._pedalsController = true;

        if (_collider != null)
            _collider.enabled = false;

        if (_carMovement._wheelController && _carMovement.cameraManager != null)
            _carMovement.cameraManager.ChangeCamera();

        print("Pedals enabled");
    }

    private void OnDisable()
    {
        if (_carMovement != null)
            _carMovement._pedalsController = false;
    }

    public override void Update()
    {
        if (_carMovement == null)
            return;

        if (CheckExitInput())
        {
            SwitchController();
            return;
        }

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

    private bool CheckExitInput()
    {
        if (isKeyboard)
            return Input.GetKeyDown(exitVehicleKey);

        if (gamepad == null)
            gamepad = GetGamepadByIndex(indexGamepad);

        if (gamepad == null)
            return false;

        return gamepad.buttonNorth.wasPressedThisFrame || gamepad.buttonEast.wasPressedThisFrame;
    }

    public override void SwitchController()
    {
        if (_carMovement == null)
            return;

        if (_collider != null)
            _collider.enabled = true;

        _carMovement.forwardInput = false;
        _carMovement.backwardInput = false;
        _carMovement._pedalsController = false;

        if (_carMovement._wheelController && _carMovement.cameraManager != null)
            _carMovement.cameraManager.ChangeCamera();

        RestorePlayer();

        base.SwitchController();
    }

    private void RestorePlayer()
    {
        if (controllerToSwitch == null)
        {
            Debug.LogWarning(gameObject.name + " : controllerToSwitch null, impossible de restaurer le joueur.");
            return;
        }

        GameObject playerObject = controllerToSwitch.gameObject;

        playerObject.transform.SetParent(null, true);
        playerObject.transform.position = transform.position + transform.right * 2f;

        PlayerInput input = playerObject.GetComponent<PlayerInput>();
        if (input != null)
            input.enabled = true;

        PlayerControllerMovement movement = playerObject.GetComponent<PlayerControllerMovement>();
        if (movement != null)
            movement.enabled = true;

        Rigidbody rb = playerObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Collider[] colliders = playerObject.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            if (!col.isTrigger)
                col.enabled = true;
        }

        Renderer[] renderers = playerObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
            rend.enabled = true;
    }

    public override void PositiveForce()
    {
        if (_carMovement != null)
            _carMovement.forwardInput = true;
    }

    public override void NegativeForce()
    {
        if (_carMovement != null)
            _carMovement.backwardInput = true;
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
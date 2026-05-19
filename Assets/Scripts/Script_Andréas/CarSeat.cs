using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum CarSeatType
{
    Wheel,
    Pedals
}

public class CarSeat : MonoBehaviour
{
    [Header("Seat")]
    [SerializeField] private CarSeatType seatType;
    [SerializeField] private CarMovement carMovement;
    [SerializeField] private Controller vehicleController;

    [Header("Exit")]
    [SerializeField] private KeyCode keyboardExitKey = KeyCode.E;
    [SerializeField] private float exitInputLockDuration = 0.5f;
    [SerializeField] private float exitDistance = 2f;
    [SerializeField] private Vector3 customExitDirection = Vector3.right;

    [Header("Events")]
    public UnityEvent OnSeatEntered;
    public UnityEvent OnSeatExited;

    private GameObject occupant;
    private PlayerInput occupantInput;
    private PlayerControllerMovement occupantMovement;
    private Rigidbody occupantRb;

    private string storedControlScheme;
    private string storedActionMap;
    private InputDevice[] storedDevices;

    private bool occupantUsesKeyboard;
    private int occupantGamepadIndex = -1;
    private float exitInputUnlockedTime;

    public bool IsOccupied => occupant != null;
    public CarSeatType SeatType => seatType;

    private void Awake()
    {
        if (carMovement == null)
            carMovement = GetComponentInParent<CarMovement>();

        if (vehicleController == null)
            vehicleController = GetComponent<Controller>();
    }

    private void Update()
    {
        if (!IsOccupied)
            return;

        if (Time.time < exitInputUnlockedTime)
            return;

        if (HasExitInput())
            ExitSeat();
    }

    public bool TryEnterSeat(GameObject playerRoot)
    {
        if (playerRoot == null)
            return false;

        if (IsOccupied)
            return false;

        if (carMovement == null || vehicleController == null)
            return false;

        EnterSeat(playerRoot);
        return true;
    }

    public void ForceExitSeat()
    {
        ExitSeat();
    }

    private void EnterSeat(GameObject playerRoot)
    {
        occupant = playerRoot;

        CacheOccupantComponents();
        StoreOccupantInputData();

        HideOccupant();

        occupant.transform.SetParent(transform, true);

        if (vehicleController != null)
        {
            vehicleController.isKeyboard = occupantUsesKeyboard;
            vehicleController.indexGamepad = occupantGamepadIndex;
            vehicleController.RefreshInputDevice();
            vehicleController.enabled = true;
        }

        if (carMovement != null)
            carMovement.PlayerEnter(seatType == CarSeatType.Wheel);

        exitInputUnlockedTime = Time.time + exitInputLockDuration;

        OnSeatEntered?.Invoke();
    }

    private void ExitSeat()
    {
        if (occupant == null)
            return;

        if (carMovement != null)
            carMovement.PlayerExit(seatType == CarSeatType.Wheel);

        occupant.transform.SetParent(null, true);
        occupant.transform.position = GetExitPosition();

        RestoreOccupant();

        if (vehicleController != null)
            vehicleController.enabled = false;

        OnSeatExited?.Invoke();

        ClearOccupant();
    }

    private Vector3 GetExitPosition()
    {
        Vector3 direction = transform.TransformDirection(customExitDirection.normalized);
        return transform.position + direction * exitDistance;
    }

    private bool HasExitInput()
    {
        if (occupantUsesKeyboard)
            return Input.GetKeyDown(keyboardExitKey);

        if (occupantGamepadIndex < 0 || occupantGamepadIndex >= Gamepad.all.Count)
            return false;

        Gamepad pad = Gamepad.all[occupantGamepadIndex];

        if (pad == null)
            return false;

        return pad.dpad.down.wasPressedThisFrame;
    }

    private void CacheOccupantComponents()
    {
        occupantInput = occupant.GetComponent<PlayerInput>();
        occupantMovement = occupant.GetComponent<PlayerControllerMovement>();
        occupantRb = occupant.GetComponent<Rigidbody>();
    }

    private void StoreOccupantInputData()
    {
        storedControlScheme = null;
        storedActionMap = null;
        storedDevices = null;

        occupantUsesKeyboard = true;
        occupantGamepadIndex = -1;

        if (occupantInput == null)
            return;

        storedControlScheme = occupantInput.currentControlScheme;

        if (occupantInput.currentActionMap != null)
            storedActionMap = occupantInput.currentActionMap.name;
        else
            storedActionMap = occupantInput.defaultActionMap;

        storedDevices = new InputDevice[occupantInput.devices.Count];

        for (int i = 0; i < occupantInput.devices.Count; i++)
            storedDevices[i] = occupantInput.devices[i];

        DetectOccupantDevice();
    }

    private void DetectOccupantDevice()
    {
        if (occupantInput == null)
            return;

        string scheme = occupantInput.currentControlScheme;

        if (!string.IsNullOrEmpty(scheme))
        {
            string lowerScheme = scheme.ToLower();

            if (lowerScheme.Contains("keyboard") || lowerScheme.Contains("mouse"))
            {
                occupantUsesKeyboard = true;
                occupantGamepadIndex = -1;
                return;
            }
        }

        foreach (InputDevice device in occupantInput.devices)
        {
            if (device is Keyboard || device is Mouse)
            {
                occupantUsesKeyboard = true;
                occupantGamepadIndex = -1;
                return;
            }

            if (device is Gamepad gamepadDevice)
            {
                for (int i = 0; i < Gamepad.all.Count; i++)
                {
                    if (Gamepad.all[i] == gamepadDevice)
                    {
                        occupantUsesKeyboard = false;
                        occupantGamepadIndex = i;
                        return;
                    }
                }
            }
        }

        occupantUsesKeyboard = true;
        occupantGamepadIndex = -1;
    }

    private void HideOccupant()
    {
        if (occupantRb != null)
        {
            occupantRb.linearVelocity = Vector3.zero;
            occupantRb.angularVelocity = Vector3.zero;
            occupantRb.isKinematic = true;
        }

        if (occupantInput != null)
            occupantInput.DeactivateInput();

        if (occupantMovement != null)
        {
            occupantMovement.direction = Vector2.zero;
            occupantMovement.enabled = false;
        }

        foreach (Collider col in occupant.GetComponentsInChildren<Collider>())
        {
            if (!col.isTrigger)
                col.enabled = false;
        }

        foreach (Renderer renderer in occupant.GetComponentsInChildren<Renderer>())
            renderer.enabled = false;
    }

    private void RestoreOccupant()
    {
        if (occupantRb != null)
        {
            occupantRb.isKinematic = false;
            occupantRb.linearVelocity = Vector3.zero;
            occupantRb.angularVelocity = Vector3.zero;
        }

        foreach (Collider col in occupant.GetComponentsInChildren<Collider>())
        {
            if (!col.isTrigger)
                col.enabled = true;
        }

        foreach (Renderer renderer in occupant.GetComponentsInChildren<Renderer>())
            renderer.enabled = true;

        if (occupantInput != null)
        {
            occupantInput.ActivateInput();

            if (!string.IsNullOrEmpty(storedControlScheme) &&
                storedDevices != null &&
                storedDevices.Length > 0)
            {
                occupantInput.SwitchCurrentControlScheme(
                    storedControlScheme,
                    storedDevices
                );
            }

            if (!string.IsNullOrEmpty(storedActionMap))
                occupantInput.SwitchCurrentActionMap(storedActionMap);
        }

        if (occupantMovement != null)
        {
            occupantMovement.direction = Vector2.zero;
            occupantMovement.enabled = true;
        }
    }

    private void ClearOccupant()
    {
        occupant = null;
        occupantInput = null;
        occupantMovement = null;
        occupantRb = null;

        storedControlScheme = null;
        storedActionMap = null;
        storedDevices = null;

        occupantUsesKeyboard = true;
        occupantGamepadIndex = -1;
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerVehicleInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject playerRoot;

    private CarMovement currentCar;
    private Controller currentVehicleController;

    private bool canEnterVehicle;
    private bool isWheelSide;

    private PlayerInput playerInput;
    private Rigidbody rb;
    private Controller playerController;

    private void Awake()
    {
        if (playerRoot == null)
            playerRoot = gameObject;

        playerInput = playerRoot.GetComponent<PlayerInput>();
        rb = playerRoot.GetComponent<Rigidbody>();
        playerController = playerRoot.GetComponent<Controller>();
    }

    public bool TryEnterVehicle()
    {
        if (!canEnterVehicle)
            return false;

        if (currentCar == null)
            return false;

        if (currentVehicleController == null)
            return false;

        EnterVehicle();
        return true;
    }

    private void EnterVehicle()
    {
        VehicleSeatHandler seatHandler = currentVehicleController.GetComponent<VehicleSeatHandler>();

        if (seatHandler == null)
        {
            Debug.LogWarning("Aucun VehicleSeatHandler trouvé sur la porte.");
            return;
        }

        seatHandler.EnterSeat(playerRoot);
    }

    private void ApplyInputToVehicleController()
    {
        if (playerController != null)
        {
            currentVehicleController.isKeyboard = playerController.isKeyboard;
            currentVehicleController.indexGamepad = playerController.indexGamepad;
            currentVehicleController.controllerToSwitch = playerController;
        }

        PlayerInput vehicleInput = currentVehicleController.GetComponent<PlayerInput>();

        if (playerInput == null || vehicleInput == null)
            return;

        if (playerInput.devices.Count == 0)
            return;

        InputDevice device = playerInput.devices[0];

        vehicleInput.SwitchCurrentControlScheme(
            playerInput.currentControlScheme,
            device
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftDoor"))
        {
            TryCacheVehicleDoor(other, true);
        }
        else if (other.CompareTag("RightDoor"))
        {
            TryCacheVehicleDoor(other, false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LeftDoor") && isWheelSide)
            ClearVehicleDoor();

        if (other.CompareTag("RightDoor") && !isWheelSide)
            ClearVehicleDoor();
    }

    private void TryCacheVehicleDoor(Collider other, bool wheelSide)
    {
        CarMovement car = other.GetComponentInParent<CarMovement>();

        if (car == null)
            return;

        Controller vehicleController = other.GetComponent<Controller>();

        if (vehicleController == null)
            vehicleController = other.GetComponentInParent<Controller>();

        if (vehicleController == null)
        {
            Debug.LogWarning(other.name + " : aucun Controller trouvé sur la porte voiture.");
            return;
        }

        currentCar = car;
        currentVehicleController = vehicleController;
        isWheelSide = wheelSide;
        canEnterVehicle = true;
    }

    private void ClearVehicleDoor()
    {
        currentCar = null;
        currentVehicleController = null;
        canEnterVehicle = false;
    }
}
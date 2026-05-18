using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleExitEventRelay : MonoBehaviour
{
    [SerializeField] private VehicleSeatHandler seatHandler;

    private void Awake()
    {
        if (seatHandler == null)
            seatHandler = GetComponent<VehicleSeatHandler>();
    }

    public void OnExitVehicle(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (seatHandler != null)
            seatHandler.ExitSeat();
    }
}
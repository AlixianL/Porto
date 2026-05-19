using UnityEngine;

public class PlayerVehicleInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject playerRoot;

    private CarSeat currentSeat;
    private bool canEnterVehicle;

    private void Awake()
    {
        if (playerRoot == null)
            playerRoot = gameObject;
    }

    public bool TryEnterVehicle()
    {
        if (!canEnterVehicle)
            return false;

        if (currentSeat == null)
            return false;

        return currentSeat.TryEnterSeat(playerRoot);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("LeftDoor") && !other.CompareTag("RightDoor"))
            return;

        CarSeat seat = other.GetComponent<CarSeat>();

        if (seat == null)
            seat = other.GetComponentInParent<CarSeat>();

        if (seat == null)
            return;

        if (seat.IsOccupied)
            return;

        currentSeat = seat;
        canEnterVehicle = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("LeftDoor") && !other.CompareTag("RightDoor"))
            return;

        CarSeat seat = other.GetComponent<CarSeat>();

        if (seat == null)
            seat = other.GetComponentInParent<CarSeat>();

        if (seat != null && seat == currentSeat)
        {
            currentSeat = null;
            canEnterVehicle = false;
        }
    }
}
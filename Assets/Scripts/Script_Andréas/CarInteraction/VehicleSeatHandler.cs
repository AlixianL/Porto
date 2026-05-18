using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleSeatHandler : MonoBehaviour
{
    [SerializeField] private CarMovement carMovement;
    [SerializeField] private Controller vehicleController;
    [SerializeField] private bool isWheelSeat;
    [SerializeField] private Vector3 exitOffset = new Vector3(2f, 0f, 0f);

    private GameObject currentPlayer;
    private PlayerInput playerInput;
    private PlayerControllerMovement playerMovement;
    private Rigidbody playerRb;

    private void Awake()
    {
        if (carMovement == null)
            carMovement = GetComponentInParent<CarMovement>();

        if (vehicleController == null)
            vehicleController = GetComponent<Controller>();
    }

    public void EnterSeat(GameObject playerRoot)
    {
        currentPlayer = playerRoot;

        playerInput = currentPlayer.GetComponent<PlayerInput>();
        playerMovement = currentPlayer.GetComponent<PlayerControllerMovement>();
        playerRb = currentPlayer.GetComponent<Rigidbody>();

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.isKinematic = true;
        }

        if (playerMovement != null)
            playerMovement.enabled = false;

        SetPlayerVisible(false);
        SetPlayerCollisions(false);

        currentPlayer.transform.SetParent(transform, true);

        if (playerInput != null)
            playerInput.enabled = false;

        if (vehicleController != null)
            vehicleController.enabled = true;

        if (carMovement != null)
            carMovement.PlayerEnter(isWheelSeat);
    }

    public void ExitSeat()
    {
        if (currentPlayer == null)
            return;

        if (carMovement != null)
            carMovement.PlayerExit(isWheelSeat);

        currentPlayer.transform.SetParent(null, true);
        currentPlayer.transform.position = transform.position + transform.TransformDirection(exitOffset);

        if (playerRb != null)
        {
            playerRb.isKinematic = false;
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        SetPlayerVisible(true);
        SetPlayerCollisions(true);

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerInput != null)
            playerInput.enabled = true;

        if (vehicleController != null)
            vehicleController.enabled = false;

        currentPlayer = null;
        playerInput = null;
        playerMovement = null;
        playerRb = null;
    }

    private void SetPlayerVisible(bool state)
    {
        Renderer[] renderers = currentPlayer.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
            renderer.enabled = state;
    }

    private void SetPlayerCollisions(bool state)
    {
        Collider[] colliders = currentPlayer.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            if (!col.isTrigger)
                col.enabled = state;
        }
    }
}
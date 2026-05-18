using UnityEngine;

public class PlayerInteractionDispatcher : MonoBehaviour
{
    [Header("Gameplay Systems")]
    [SerializeField] private TwoPlayerCarryInteractor twoPlayerCarryInteractor;
    [SerializeField] private ObjectThrower objectThrower;
    [SerializeField] private PlayerCarryThrower playerCarryThrower;
    [SerializeField] private PlayerVehicleInteractor vehicleInteractor;

    private void Awake()
    {
        if (twoPlayerCarryInteractor == null)
            twoPlayerCarryInteractor = GetComponent<TwoPlayerCarryInteractor>();

        if (objectThrower == null)
            objectThrower = GetComponent<ObjectThrower>();

        if (playerCarryThrower == null)
            playerCarryThrower = GetComponent<PlayerCarryThrower>();

        if (vehicleInteractor == null)
            vehicleInteractor = GetComponent<PlayerVehicleInteractor>();
    }

    public void HandleInteract()
    {
        if (twoPlayerCarryInteractor != null && twoPlayerCarryInteractor.TryHeavyCarryAction())
            return;

        if (vehicleInteractor != null && vehicleInteractor.TryEnterVehicle())
            return;

        if (objectThrower != null && objectThrower.TryObjectAction())
            return;

        if (playerCarryThrower != null)
            playerCarryThrower.TryCarryAction();
    }
}
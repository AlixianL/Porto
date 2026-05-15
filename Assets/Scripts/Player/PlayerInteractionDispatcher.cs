using UnityEngine;

public class PlayerInteractionDispatcher : MonoBehaviour
{
    [Header("Gameplay Systems")]
    [SerializeField] private TwoPlayerCarryInteractor twoPlayerCarryInteractor;
    [SerializeField] private ObjectThrower objectThrower;
    [SerializeField] private PlayerCarryThrower playerCarryThrower;

    private void Awake()
    {
        if (twoPlayerCarryInteractor == null)
            twoPlayerCarryInteractor = GetComponent<TwoPlayerCarryInteractor>();

        if (objectThrower == null)
            objectThrower = GetComponent<ObjectThrower>();

        if (playerCarryThrower == null)
            playerCarryThrower = GetComponent<PlayerCarryThrower>();
    }

    public void HandleInteract()
    {
        if (twoPlayerCarryInteractor != null && twoPlayerCarryInteractor.TryHeavyCarryAction())
            return;

        if (objectThrower != null && objectThrower.TryObjectAction())
            return;

        if (playerCarryThrower != null)
            playerCarryThrower.TryCarryAction();
    }
}
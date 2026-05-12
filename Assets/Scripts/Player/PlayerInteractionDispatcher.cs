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
        Debug.Log("DISPATCHER INTERACT : " + gameObject.name);

        if (twoPlayerCarryInteractor != null && twoPlayerCarryInteractor.TryHeavyCarryAction())
        {
            Debug.Log("Interaction prise par objet lourd");
            return;
        }

        if (objectThrower != null && objectThrower.TryObjectAction())
        {
            Debug.Log("Interaction prise par objet simple");
            return;
        }

        if (playerCarryThrower != null && playerCarryThrower.TryCarryAction())
        {
            Debug.Log("Interaction prise par joueur porté");
            return;
        }

        Debug.Log("Aucune interaction trouvée");
    }
}
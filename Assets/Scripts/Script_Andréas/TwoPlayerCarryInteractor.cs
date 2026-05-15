using UnityEngine;

public class TwoPlayerCarryInteractor : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private LayerMask heavyObjectLayer;

    public TwoPlayerCarryObject CurrentHeavyObject { get; private set; }

    private PlayerMovement playerMovement;
    private PlayerControllerMovement playerControllerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerControllerMovement = GetComponent<PlayerControllerMovement>();
    }

    public bool TryHeavyCarryAction()
    {
        if (CurrentHeavyObject != null)
        {
            CurrentHeavyObject.RequestAction(this);
            return true;
        }

        TwoPlayerCarryObject heavyObject = FindNearestHeavyObject();

        if (heavyObject == null)
            return false;

        bool registered = heavyObject.TryRegisterPlayer(this);

        if (registered)
            CurrentHeavyObject = heavyObject;

        return registered;
    }

    private TwoPlayerCarryObject FindNearestHeavyObject()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            interactRange,
            heavyObjectLayer
        );

        if (hits.Length == 0)
            return null;

        TwoPlayerCarryObject nearestObject = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            TwoPlayerCarryObject heavyObject =
                hit.GetComponentInParent<TwoPlayerCarryObject>();

            if (heavyObject == null)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                heavyObject.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestObject = heavyObject;
            }
        }

        return nearestObject;
    }

    public Vector3 GetInputDirection()
    {
        Vector2 input = Vector2.zero;

        if (playerMovement != null)
            input = playerMovement.direction;
        else if (playerControllerMovement != null)
            input = playerControllerMovement.direction;

        Vector3 direction = new Vector3(input.x, 0f, input.y);

        if (direction.sqrMagnitude < 0.01f)
            direction = transform.forward;

        direction.y = 0f;
        return direction.normalized;
    }

    public void ClearHeavyObject(TwoPlayerCarryObject heavyObject)
    {
        if (CurrentHeavyObject == heavyObject)
            CurrentHeavyObject = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
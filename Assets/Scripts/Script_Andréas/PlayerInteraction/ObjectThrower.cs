using UnityEngine;

public class ObjectThrower : MonoBehaviour
{
    [Header("Throw")]
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForwardForce = 6.5f;
    [SerializeField] private float throwUpForce = 4.8f;

    [Header("Pickup")]
    [SerializeField] private float pickupRange = 1.5f;
    [SerializeField] private LayerMask pickupLayer;

    [Header("Input Protection")]
    [SerializeField] private float actionCooldown = 0.25f;

    [Header("References")]
    [SerializeField] private ThrowDirection throwDirection;
    
    private ThrowLandingPreview landingPreview;

    private float nextActionTime;

    private GameObject heldObject;
    private Rigidbody heldRb;
    private Collider heldCol;
    private Vector3 heldOriginalScale = Vector3.one;

    private void Awake()
    {
        landingPreview = GetComponent<ThrowLandingPreview>();
    }

    private void LateUpdate()
    {
        if (heldObject == null)
            return;

        heldObject.transform.position = throwPoint.position;
        heldObject.transform.rotation = throwPoint.rotation;
        heldObject.transform.localScale = heldOriginalScale;

        if (landingPreview != null)
            landingPreview.ShowPreview(throwPoint.position, GetThrowVelocity());
    }

    public void TryObjectAction()
    {
        if (Time.time < nextActionTime)
            return;

        nextActionTime = Time.time + actionCooldown;

        if (heldObject == null)
            TryPickObject();
        else
            ThrowObject();
    }

    private void TryPickObject()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, pickupLayer);

        if (hits.Length == 0)
            return;

        ThrowableObject throwable = hits[0].GetComponent<ThrowableObject>();

        if (throwable != null && throwable.IsSnapped)
            return;

        PickObject(hits[0].gameObject);
    }

    private void PickObject(GameObject obj)
    {
        ThrowableObject throwable = obj.GetComponent<ThrowableObject>();

        if (throwable != null && throwable.IsSnapped)
            return;

        heldObject = obj;
        heldRb = obj.GetComponent<Rigidbody>();
        heldCol = obj.GetComponent<Collider>();

        if (heldRb == null)
        {
            heldObject = null;
            return;
        }

        heldOriginalScale = obj.transform.localScale;

        heldRb.linearVelocity = Vector3.zero;
        heldRb.angularVelocity = Vector3.zero;
        heldRb.isKinematic = true;
        heldRb.detectCollisions = false;

        if (heldCol != null)
            heldCol.enabled = false;

        obj.transform.SetParent(throwPoint, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = heldOriginalScale;

        TutorialManager.Instance?.ValidateGrabObject();
    }

    private void ThrowObject()
    {
        heldObject.transform.SetParent(null, true);
        heldObject.transform.localScale = heldOriginalScale;

        if (heldCol != null)
            heldCol.enabled = true;

        heldRb.detectCollisions = true;
        heldRb.isKinematic = false;
        heldRb.linearVelocity = Vector3.zero;
        heldRb.angularVelocity = Vector3.zero;

        heldRb.linearVelocity = GetThrowVelocity();

        if (landingPreview != null)
            landingPreview.HidePreview();

        TutorialManager.Instance?.ValidateThrowObject();

        ClearHeldObject();
    }

    private Vector3 GetThrowVelocity()
    {
        Vector3 flatDirection = throwDirection != null
            ? throwDirection.GetDirection()
            : throwPoint.forward;

        flatDirection.y = 0f;

        if (flatDirection.sqrMagnitude < 0.01f)
            flatDirection = transform.forward;

        flatDirection.Normalize();

        Vector3 velocity = flatDirection * throwForwardForce;
        velocity.y = throwUpForce;

        return velocity;
    }

    public void ForceDropAndRespawnHeldObject()
    {
        if (heldObject == null)
            return;

        GameObject objectToRespawn = heldObject;
        Rigidbody rbToRespawn = heldRb;
        Collider colToRespawn = heldCol;

        objectToRespawn.transform.SetParent(null, true);

        if (colToRespawn != null)
            colToRespawn.enabled = true;

        if (rbToRespawn != null)
        {
            rbToRespawn.isKinematic = false;
            rbToRespawn.detectCollisions = true;
            rbToRespawn.linearVelocity = Vector3.zero;
            rbToRespawn.angularVelocity = Vector3.zero;
        }

        ClearHeldObject();

        RespawnableObject respawnable = objectToRespawn.GetComponent<RespawnableObject>();

        if (respawnable != null)
            respawnable.Respawn();

        if (landingPreview != null)
            landingPreview.HidePreview();
    }

    private void ClearHeldObject()
    {
        heldObject = null;
        heldRb = null;
        heldCol = null;
        heldOriginalScale = Vector3.one;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
using UnityEngine;

public class ObjectThrower : MonoBehaviour
{
    [Header("Throw")]
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForce = 10f;

    [Header("Pickup")]
    [SerializeField] private float pickupRange = 1.5f;
    [SerializeField] private LayerMask pickupLayer;

    [Header("Input Protection")]
    [SerializeField] private float actionCooldown = 0.25f;

    private float nextActionTime;

    private GameObject heldObject;
    private Rigidbody heldRb;
    private Collider heldCol;

    private Vector3 heldOriginalScale;

    [SerializeField] private ThrowDirection throwDirection;

    private void LateUpdate()
    {
        if (heldObject != null)
        {
            heldObject.transform.position = throwPoint.position;
            heldObject.transform.rotation = throwPoint.rotation;

            // Sécurité anti-scale bizarre au grab
            heldObject.transform.localScale = heldOriginalScale;
        }
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
        {
            Debug.Log("Aucun objet ramassable proche");
            return;
        }

        ThrowableObject throwable = hits[0].GetComponent<ThrowableObject>();

        if (throwable != null && throwable.IsSnapped)
        {
            Debug.Log("Objet déjà placé, non récupérable");
            return;
        }

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
            Debug.LogError("Objet sans Rigidbody : " + obj.name);
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

        Debug.Log("Objet ramassé : " + obj.name);
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

        Vector3 direction;

        if (throwDirection != null)
            direction = throwDirection.GetDirection() + Vector3.up * 0.3f;
        else
            direction = throwPoint.forward + Vector3.up * 0.3f;

        direction.Normalize();

        heldRb.AddForce(direction * throwForce, ForceMode.Impulse);

        TutorialManager.Instance?.ValidateThrowObject();

        Debug.Log("Objet lancé vers : " + direction);

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

    public void ForceDropAndRespawnHeldObject()
    {
        if (heldObject == null)
            return;

        GameObject objectToRespawn = heldObject;
        Rigidbody rbToRespawn = heldRb;
        Collider colToRespawn = heldCol;

        heldObject = null;
        heldRb = null;
        heldCol = null;

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

        RespawnableObject respawnable = objectToRespawn.GetComponent<RespawnableObject>();

        if (respawnable != null)
            respawnable.Respawn();
    }
}
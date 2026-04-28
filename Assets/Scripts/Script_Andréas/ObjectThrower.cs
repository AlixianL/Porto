using UnityEngine;

public class ObjectThrower : MonoBehaviour
{
    [Header("Throw")]
    public Transform throwPoint;
    public float throwForce = 10f;

    [Header("Pickup")]
    public float pickupRange = 1.5f;
    public LayerMask pickupLayer;

    private GameObject heldObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
                TryPickObject();
            else
                ThrowObject();
        }
    }

    void LateUpdate()
    {
        if (heldObject != null)
        {
            heldObject.transform.position = throwPoint.position;
            heldObject.transform.rotation = throwPoint.rotation;
        }
    }

    void TryPickObject()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pickupRange, pickupLayer);

        if (hits.Length == 0)
        {
            Debug.Log("Aucun objet ramassable proche");
            return;
        }

        GameObject obj = hits[0].gameObject;
        PickObject(obj);
    }

    void PickObject(GameObject obj)
    {
        heldObject = obj;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        Collider col = obj.GetComponent<Collider>();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.detectCollisions = false;

        if (col != null)
            col.enabled = false;

        obj.transform.SetParent(throwPoint, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        Debug.Log("Objet ramassé : " + obj.name);
    }

    void ThrowObject()
    {
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        Collider col = heldObject.GetComponent<Collider>();

        heldObject.transform.SetParent(null, true);

        if (col != null)
            col.enabled = true;

        rb.detectCollisions = true;
        rb.isKinematic = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 throwDirection = throwPoint.forward + Vector3.up * 0.5f;
        throwDirection.Normalize();

        rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);

        Debug.Log("Objet lancé");

        heldObject = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}

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
    private Rigidbody heldRb;
    private Collider heldCol;
    [SerializeField] private ThrowDirection throwDirection;



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

    void TryPickObject()
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

    void PickObject(GameObject obj)
    {
        ThrowableObject throwable = obj.GetComponent<ThrowableObject>();

        if (throwable != null && throwable.IsSnapped)
            return;
        heldObject = obj;
        heldRb = obj.GetComponent<Rigidbody>();
        heldCol = obj.GetComponent<Collider>();
        // Désactiver la physique
        heldRb.linearVelocity = Vector3.zero;
        heldRb.angularVelocity = Vector3.zero;
        heldRb.isKinematic = true;
        if (heldCol != null)
            heldCol.enabled = false;
        // Parenter directement — PAS de repositionnement manuel dans LateUpdate
        obj.transform.SetParent(throwPoint, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        Debug.Log("Objet ramassé : " + obj.name);
    }

    void ThrowObject()
    {
        // Départenter AVANT de réactiver la physique
        heldObject.transform.SetParent(null);
        if (heldCol != null)
            heldCol.enabled = true;
        heldRb.isKinematic = false;
        heldRb.linearVelocity = Vector3.zero;
        heldRb.angularVelocity = Vector3.zero;
        // Direction du lancer
        Vector3 direction = (throwDirection.GetDirection() + Vector3.up * 0.3f).normalized;
        heldRb.AddForce(direction * throwForce, ForceMode.Impulse);
        Debug.Log("Objet lancé vers " + direction);
        heldObject = null;
        heldRb = null;
        heldCol = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }

    void LateUpdate()
    {
        if (heldObject != null)
        {
            heldObject.transform.position = throwPoint.position;
            heldObject.transform.rotation = throwPoint.rotation;
        }
    }
}
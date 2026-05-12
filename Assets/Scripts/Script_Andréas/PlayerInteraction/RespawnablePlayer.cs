using UnityEngine;

public class RespawnablePlayer : MonoBehaviour
{
    [SerializeField] private float deathY = -5f;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;
    private Collider col;
    private ObjectThrower objectThrower;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        objectThrower = GetComponent<ObjectThrower>();

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void Update()
    {
        if (transform.position.y <= deathY)
            Respawn();
    }

    public void Respawn()
    {
        if (objectThrower != null)
            objectThrower.ForceDropAndRespawnHeldObject();

        transform.SetParent(null, true);
        transform.position = startPosition;
        transform.rotation = startRotation;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (col != null)
            col.enabled = true;
    }
}
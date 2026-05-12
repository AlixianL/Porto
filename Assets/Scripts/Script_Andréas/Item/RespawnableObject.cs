using UnityEngine;

public class RespawnableObject : MonoBehaviour
{
    [SerializeField] private float deathY = -5f;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        ThrowableObject throwable = GetComponent<ThrowableObject>();

        if (throwable != null && throwable.IsSnapped)
            return;

        if (transform.position.y <= deathY)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
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

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        Debug.Log(gameObject.name + " respawn");
    }
}
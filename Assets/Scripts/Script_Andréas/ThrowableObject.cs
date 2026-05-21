using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    public bool IsSnapped { get; private set; }

    private Rigidbody rb;
    private Collider col;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void SnapTo(Transform snapPoint)
    {
        IsSnapped = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;

        transform.SetParent(snapPoint, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (col != null)
            col.enabled = true;
    }
}
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    public bool IsSnapped { get; private set; }

    public void SetSnapped(bool snapped)
    {
        IsSnapped = snapped;
    }

    public void DisablePhysicsAfterSnap()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
            col.enabled = false;
    }
}
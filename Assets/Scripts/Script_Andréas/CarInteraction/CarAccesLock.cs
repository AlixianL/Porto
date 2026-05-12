using UnityEngine;

public class CarAccessLock : MonoBehaviour
{
    [SerializeField] private Collider[] carEntryColliders;

    private void Start()
    {
        LockCarAccess();
    }

    public void LockCarAccess()
    {
        SetEntryColliders(false);
    }

    public void UnlockCarAccess()
    {
        SetEntryColliders(true);
    }

    private void SetEntryColliders(bool state)
    {
        foreach (Collider entryCollider in carEntryColliders)
        {
            if (entryCollider != null)
                entryCollider.enabled = state;
        }
    }
}
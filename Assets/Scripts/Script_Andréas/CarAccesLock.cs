using UnityEngine;

public class CarAccessLock : MonoBehaviour
{
    [Header("Required snapped objects")]
    [SerializeField] private ThrowableObject[] requiredObjects;

    [Header("Car entry colliders")]
    [SerializeField] private Collider[] carEntryColliders;

    private bool carUnlocked;

    private void Start()
    {
        SetCarEntry(false);
    }

    private void Update()
    {
        if (carUnlocked)
            return;

        if (AreAllObjectsSnapped())
        {
            carUnlocked = true;
            SetCarEntry(true);

            Debug.Log("VOITURE DÉVERROUILLÉE : toutes les valises sont placées.");
        }
    }

    private bool AreAllObjectsSnapped()
    {
        if (requiredObjects == null || requiredObjects.Length == 0)
            return false;

        foreach (ThrowableObject obj in requiredObjects)
        {
            if (obj == null)
                return false;

            if (!obj.IsSnapped)
                return false;
        }

        return true;
    }

    private void SetCarEntry(bool state)
    {
        foreach (Collider col in carEntryColliders)
        {
            if (col != null)
                col.enabled = state;
        }
    }
}
using UnityEngine;
using UnityEngine.Events;

public class SnapZone : MonoBehaviour
{
    [Header("Snap Points")]
    [SerializeField] private Transform[] snapPoints;

    [Header("Events")]
    public UnityEvent OnObjectSnapped;

    private bool[] occupied;

    private void Awake()
    {
        occupied = new bool[snapPoints.Length];
    }

    private void OnTriggerEnter(Collider other)
    {
        ThrowableObject throwable =
            other.GetComponentInParent<ThrowableObject>();

        if (throwable == null)
            return;

        if (throwable.IsSnapped)
            return;

        int freeSlotIndex = GetFreeSlotIndex();

        if (freeSlotIndex == -1)
            return;

        Transform targetSnapPoint = snapPoints[freeSlotIndex];

        throwable.transform.position = targetSnapPoint.position;
        throwable.transform.rotation = targetSnapPoint.rotation;
        throwable.transform.SetParent(targetSnapPoint);

        throwable.SetSnapped(true);
        throwable.DisablePhysicsAfterSnap();

        occupied[freeSlotIndex] = true;

        OnObjectSnapped?.Invoke();
    }

    private int GetFreeSlotIndex()
    {
        for (int i = 0; i < occupied.Length; i++)
        {
            if (!occupied[i])
                return i;
        }

        return -1;
    }
}
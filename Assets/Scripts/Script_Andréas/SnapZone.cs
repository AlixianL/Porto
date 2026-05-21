using UnityEngine;

public class SnapZone : MonoBehaviour
{
    [SerializeField] private Transform[] snapPoints;
    [SerializeField] private PuzzleValidator puzzleValidator;

    private bool[] occupied;

    void Awake()
    {
        occupied = new bool[snapPoints.Length];
    }

    void OnTriggerEnter(Collider other)
    {
        ThrowableObject throwable = other.GetComponent<ThrowableObject>();

        if (throwable == null || throwable.IsSnapped)
            return;

        int index = GetFreeSlot();

        if (index == -1)
            return;

        throwable.SnapTo(snapPoints[index]);
        occupied[index] = true;

        if (puzzleValidator != null)
            puzzleValidator.RegisterObjectSnapped();
    }

    int GetFreeSlot()
    {
        for (int i = 0; i < occupied.Length; i++)
        {
            if (!occupied[i])
                return i;
        }

        return -1;
    }
}
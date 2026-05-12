using UnityEngine;

public class PuzzleValidator : MonoBehaviour
{
    [SerializeField] private int requiredObjects = 4;

    private int currentObjects = 0;

    public bool IsPuzzleValidated { get; private set; }

    public void RegisterObjectSnapped()
    {
        if (IsPuzzleValidated)
            return;

        currentObjects++;

        Debug.Log("Valises placées : " + currentObjects + " / " + requiredObjects);

        if (currentObjects >= requiredObjects)
        {
            ValidatePuzzle();
        }
    }

    private void ValidatePuzzle()
    {
        IsPuzzleValidated = true;
        Debug.Log("Puzzle valises validé.");
    }

    public void DebugValidatePuzzle()
    {
        if (IsPuzzleValidated)
            return;

        currentObjects = requiredObjects;
        ValidatePuzzle();
    }
}
using UnityEngine;

public class PuzzleValidator : MonoBehaviour
{
    [SerializeField] private int requiredObjects = 3;

    private int currentObjects;

    public void RegisterObjectSnapped()
    {
        currentObjects++;

        Debug.Log("Valises placées : " + currentObjects + " / " + requiredObjects);

        if (currentObjects >= requiredObjects)
        {
            ValidatePuzzle();
        }
    }

    void ValidatePuzzle()
    {
        Debug.Log("Puzzle validé ! Passage à la phase voiture possible.");
    }
}
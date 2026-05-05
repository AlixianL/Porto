using UnityEngine;

public class PuzzleValidator : MonoBehaviour
{
    [SerializeField] private int requiredObjects = 4;

    private int currentObjects = 0;

    public bool IsPuzzleValidated { get; private set; }

    private void Start()
    {
        IsPuzzleValidated = false;
        Debug.Log("PuzzleValidator initialisé. Required Objects = " + requiredObjects);
    }

    public void RegisterObjectSnapped()
    {
        if (IsPuzzleValidated)
            return;

        currentObjects++;

        Debug.Log("Objet snap validé : " + currentObjects + " / " + requiredObjects);

        if (currentObjects >= requiredObjects)
        {
            ValidatePuzzle();
        }
    }

    private void ValidatePuzzle()
    {
        IsPuzzleValidated = true;
        Debug.Log("PUZZLE VALISES VALIDÉ : voiture accessible.");
    }
}
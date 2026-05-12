using UnityEngine;
using UnityEngine.Events;

public class ObjectiveCounter : MonoBehaviour
{
    [Header("Objective")]
    [SerializeField] private int requiredCount = 4;

    [Header("Events")]
    public UnityEvent OnProgress;
    public UnityEvent OnCompleted;

    private int currentCount;
    private bool completed;

    public bool IsCompleted => completed;

    public void AddProgress()
    {
        if (completed)
            return;

        currentCount++;

        Debug.Log("Objective progress : " + currentCount + " / " + requiredCount);

        OnProgress?.Invoke();

        if (currentCount >= requiredCount)
        {
            completed = true;
            OnCompleted?.Invoke();
        }
    }

    public void DebugComplete()
    {
        if (completed)
            return;

        currentCount = requiredCount;
        completed = true;

        Debug.Log("Objective completed by debug.");

        OnCompleted?.Invoke();
    }
}
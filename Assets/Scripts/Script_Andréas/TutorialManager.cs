using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private TutorialStepUI[] stepsUI;

    private int currentStep = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < stepsUI.Length; i++)
        {
            stepsUI[i].gameObject.SetActive(false);
            stepsUI[i].SetPending();
        }

        if (stepsUI.Length > 0)
        {
            stepsUI[0].gameObject.SetActive(true);
            stepsUI[0].SetCurrent();
        }
    }

    public void ValidateMove() => ValidateStep(0);
    public void ValidateGrabObject() => ValidateStep(1);
    public void ValidateThrowObject() => ValidateStep(2);
    public void ValidateGrabPlayer() => ValidateStep(3);
    public void ValidateThrowPlayer() => ValidateStep(4);
    public void ValidateCar() => ValidateStep(5);

    private void ValidateStep(int stepIndex)
    {
        if (stepIndex != currentStep)
            return;

        if (currentStep >= stepsUI.Length)
            return;

        stepsUI[currentStep].Validate();

        int previousStep = currentStep;
        currentStep++;

        Invoke(nameof(ShowNextStep), 0.4f);

        stepsUI[previousStep].gameObject.SetActive(false);
    }

    private void ShowNextStep()
    {
        if (currentStep >= stepsUI.Length)
            return;

        stepsUI[currentStep].gameObject.SetActive(true);
        stepsUI[currentStep].SetCurrent();
    }
}
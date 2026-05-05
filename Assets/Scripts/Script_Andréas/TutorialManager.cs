using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [SerializeField] private TutorialStepUI[] stepsUI;
    [SerializeField] private float validationDelay = 0.4f;

    private int currentStep = 0;
    private bool isTransitioning = false;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
    public void ValidatePutSuitcase() => ValidateStep(5);
    public void ValidateCarLeft() => ValidateStep(6);
    public void ValidateCarRight() => ValidateStep(7);

    private void ValidateStep(int stepIndex)
    {
        if (isTransitioning)
            return;

        if (currentStep < 0 || currentStep >= stepsUI.Length)
            return;

        if (stepIndex != currentStep)
            return;

        isTransitioning = true;

        stepsUI[currentStep].Validate();

        Invoke(nameof(HideCurrentAndShowNextStep), validationDelay);
    }

    private void HideCurrentAndShowNextStep()
    {
        if (currentStep < 0 || currentStep >= stepsUI.Length)
            return;

        stepsUI[currentStep].gameObject.SetActive(false);

        currentStep++;

        if (currentStep >= stepsUI.Length)
        {
            Debug.Log("Tutoriel terminé");
            return;
        }

        stepsUI[currentStep].gameObject.SetActive(true);
        stepsUI[currentStep].SetCurrent();

        isTransitioning = false;
    }
}
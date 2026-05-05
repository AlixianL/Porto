using UnityEngine;

public class CarTutorialValidator : MonoBehaviour
{
    [SerializeField] private CarMovement carMovement;

    [Header("Validation")]
    [SerializeField] private float steerDeadZone = 0.1f;

    private bool leftValidated;
    private bool rightValidated;

    private void Awake()
    {
        if (carMovement == null)
            carMovement = GetComponent<CarMovement>();
    }

    private void Update()
    {
        if (carMovement == null)
            return;

        CheckWheelInput();
        CheckPedalsInput();
    }

    private void CheckWheelInput()
    {
        if (leftValidated)
            return;

        if (!carMovement._wheelController)
            return;

        if (Mathf.Abs(carMovement.axis) > steerDeadZone)
        {
            leftValidated = true;
            TutorialManager.Instance?.ValidateCarLeft();
            Debug.Log("Tuto voiture gauche validé : direction volant");
        }
    }

    private void CheckPedalsInput()
    {
        if (rightValidated)
            return;

        if (!carMovement._pedalsController)
            return;

        if (carMovement.forwardInput || carMovement.backwardInput)
        {
            rightValidated = true;
            TutorialManager.Instance?.ValidateCarRight();
            Debug.Log("Tuto voiture droite validé : accélération/frein");
        }
    }
}
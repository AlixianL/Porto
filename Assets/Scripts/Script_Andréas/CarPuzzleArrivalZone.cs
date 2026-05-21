using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CarPuzzleArrivalZone : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CarMovement carMovement;
    [SerializeField] private CarSeat wheelSeat;
    [SerializeField] private CarSeat pedalsSeat;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private Camera targetLevelCamera;

    [Header("Stop Settings")]
    [SerializeField] private float stopDuration = 2f;

    [Header("Events")]
    public UnityEvent OnArrivalSequenceStarted;
    public UnityEvent OnArrivalSequenceCompleted;

    private bool sequenceStarted;

    private void Awake()
    {
        if (cameraManager == null)
            cameraManager = FindAnyObjectByType<CameraManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (sequenceStarted)
            return;

        CarMovement detectedCar = other.GetComponentInParent<CarMovement>();

        if (detectedCar == null)
            return;

        if (carMovement != null && detectedCar != carMovement)
            return;

        if (carMovement == null)
            carMovement = detectedCar;

        StartCoroutine(ArrivalRoutine());
    }

    private IEnumerator ArrivalRoutine()
    {
        sequenceStarted = true;

        OnArrivalSequenceStarted?.Invoke();

        if (carMovement != null)
            carMovement.SetInputLocked(true);

        yield return new WaitForSeconds(stopDuration);

        if (carMovement != null)
            carMovement.StopCarImmediately();

        if (wheelSeat != null)
            wheelSeat.ForceExitSeat();

        if (pedalsSeat != null)
            pedalsSeat.ForceExitSeat();

        if (cameraManager != null && targetLevelCamera != null)
            cameraManager.SetCurrentLevelCamera(targetLevelCamera);
        else
            Debug.LogWarning("CarPuzzleArrivalZone : CameraManager ou TargetLevelCamera manquant.");

        if (carMovement != null)
            carMovement.SetInputLocked(false);

        OnArrivalSequenceCompleted?.Invoke();
    }
}
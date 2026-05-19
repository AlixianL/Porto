using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Car Camera")]
    [SerializeField] private Camera carCamera;

    [Header("Current Level Camera")]
    [SerializeField] private Camera currentLevelCamera;

    private void Start()
    {
        SetCarCamera(false);
    }

    public void SetCurrentLevelCamera(Camera newLevelCamera)
    {
        if (currentLevelCamera != null)
            currentLevelCamera.enabled = false;

        currentLevelCamera = newLevelCamera;

        if (currentLevelCamera != null)
            currentLevelCamera.enabled = !IsCarCameraActive();
    }

    public void SetCarCamera(bool active)
    {
        if (carCamera != null)
            carCamera.enabled = active;

        if (currentLevelCamera != null)
            currentLevelCamera.enabled = !active;
    }

    public bool IsCarCameraActive()
    {
        return carCamera != null && carCamera.enabled;
    }
}
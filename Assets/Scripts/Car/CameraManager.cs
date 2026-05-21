using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Car Camera")]
    [SerializeField] private Camera carCamera;

    [Header("Level Cameras")]
    [SerializeField] private Camera currentLevelCamera;

    [SerializeField] private Camera[] allLevelCameras;

    private void Awake()
    {
        DisableEverything();

        if (currentLevelCamera != null)
            currentLevelCamera.enabled = true;
    }

    public void SetCurrentLevelCamera(Camera newLevelCamera)
    {
        if (newLevelCamera == null)
            return;

        DisableEverything();

        currentLevelCamera = newLevelCamera;

        currentLevelCamera.enabled = true;
    }

    public void SetCarCamera(bool active)
    {
        DisableEverything();

        if (active)
        {
            if (carCamera != null)
                carCamera.enabled = true;
        }
        else
        {
            if (currentLevelCamera != null)
                currentLevelCamera.enabled = true;
        }
    }

    private void DisableEverything()
    {
        if (carCamera != null)
            carCamera.enabled = false;

        foreach (Camera cam in allLevelCameras)
        {
            if (cam != null)
                cam.enabled = false;
        }
    }
}
using UnityEngine;

public class ThrowLandingPreview : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject landingIndicator;
    [SerializeField] private LayerMask groundLayer;

    [Header("Prediction")]
    [SerializeField] private float predictionTime = 1.2f;
    [SerializeField] private float raycastHeight = 20f;
    [SerializeField] private float raycastDistance = 40f;
    [SerializeField] private float indicatorYOffset = 0.08f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs;

    private void Awake()
    {
        HidePreview();
    }

    public void ShowPreview(Vector3 startPosition, Vector3 initialVelocity)
    {
        if (landingIndicator == null)
            return;

        Vector3 predictedPosition =
            startPosition +
            initialVelocity * predictionTime +
            0.5f * Physics.gravity * predictionTime * predictionTime;

        Vector3 rayStart = predictedPosition + Vector3.up * raycastHeight;

        Debug.DrawLine(rayStart, rayStart + Vector3.down * raycastDistance, Color.green);

        if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer))
        {
            landingIndicator.transform.position = hit.point + Vector3.up * indicatorYOffset;
            landingIndicator.transform.rotation = Quaternion.identity;
            landingIndicator.SetActive(true);

            if (showDebugLogs)
                Debug.Log("Landing preview trouvé : " + hit.point);
        }
        else
        {
            landingIndicator.SetActive(false);

            if (showDebugLogs)
                Debug.LogWarning("Landing preview : aucun sol trouvé sous le point prédit");
        }
    }

    public void HidePreview()
    {
        if (landingIndicator != null)
            landingIndicator.SetActive(false);
    }
}
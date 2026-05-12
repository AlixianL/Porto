using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCarryThrower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform carryPoint;

    [Header("Detection")]
    [SerializeField] private float grabRange = 1.5f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Carry")]
    [SerializeField] private float carryDistance = 1f;
    [SerializeField] private float carryHeight = 1.2f;

    [Header("Throw")]
    [SerializeField] private float throwForwardForce = 7f;
    [SerializeField] private float throwUpForce = 5.8f;
    [SerializeField] private float movementDisableDuration = 1f;

    [Header("Landing Preview")]
    [SerializeField] private GameObject landingIndicator;
    [SerializeField] private LayerMask landingGroundLayer;
    [SerializeField] private float landingPreviewDistanceMultiplier = 1f;
    [SerializeField] private float landingPreviewHeightOffset = 0.02f;
    [SerializeField] private float raycastStartHeight = 20f;
    [SerializeField] private float raycastDistance = 50f;

    [Header("Input Protection")]
    [SerializeField] private float actionCooldown = 0.25f;

    [Header("Events")]
    public UnityEvent OnPlayerGrabbed;
    public UnityEvent OnPlayerThrown;

    private float nextActionTime;

    private PlayerMovement carrierMovement;
    private PlayerControllerMovement carrierControllerMovement;
    private Collider carrierCol;

    private GameObject carriedPlayer;
    private Rigidbody carriedRb;
    private Collider carriedCol;

    private PlayerMovement carriedMovement;
    private PlayerControllerMovement carriedControllerMovement;

    private Vector3 lastDirection = Vector3.forward;

    private void Start()
    {
        carrierMovement = GetComponent<PlayerMovement>();
        carrierControllerMovement = GetComponent<PlayerControllerMovement>();
        carrierCol = GetComponent<Collider>();

        if (landingIndicator != null)
            landingIndicator.SetActive(false);
    }

    private void Update()
    {
        UpdateCarryDirection();
    }

    private void LateUpdate()
    {
        UpdateCarryPointPosition();

        if (carriedPlayer != null)
        {
            carriedPlayer.transform.position = carryPoint.position;
            carriedPlayer.transform.rotation = carryPoint.rotation;
        }

        UpdateLandingPreview();
    }

    public bool TryCarryAction()
    {
        if (Time.time < nextActionTime)
            return false;

        nextActionTime = Time.time + actionCooldown;

        if (carriedPlayer == null)
            return TryGrabPlayer();

        ThrowPlayer();
        return true;
    }

    private void UpdateCarryDirection()
    {
        Vector2 input = Vector2.zero;

        if (carrierMovement != null)
            input = carrierMovement.direction;
        else if (carrierControllerMovement != null)
            input = carrierControllerMovement.direction;

        Vector3 inputDirection = new Vector3(input.x, 0f, input.y);

        if (inputDirection.sqrMagnitude > 0.01f)
            lastDirection = inputDirection.normalized;
    }

    private void UpdateCarryPointPosition()
    {
        if (carryPoint == null)
            return;

        carryPoint.position =
            transform.position +
            lastDirection * carryDistance +
            Vector3.up * carryHeight;

        carryPoint.rotation = Quaternion.LookRotation(lastDirection, Vector3.up);
    }

    private bool TryGrabPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            grabRange,
            playerLayer
        );

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            GrabPlayer(hit.gameObject);
            return true;
        }

        return false;
    }

    private void GrabPlayer(GameObject target)
    {
        carriedPlayer = target;

        carriedRb = target.GetComponent<Rigidbody>();
        carriedCol = target.GetComponent<Collider>();

        carriedMovement = target.GetComponent<PlayerMovement>();
        carriedControllerMovement = target.GetComponent<PlayerControllerMovement>();

        if (carriedRb == null)
        {
            ResetCarryReferences();
            return;
        }

        if (carriedMovement != null)
            carriedMovement.enabled = false;

        if (carriedControllerMovement != null)
            carriedControllerMovement.enabled = false;

        carriedRb.linearVelocity = Vector3.zero;
        carriedRb.angularVelocity = Vector3.zero;
        carriedRb.isKinematic = true;

        if (carrierCol != null && carriedCol != null)
            Physics.IgnoreCollision(carrierCol, carriedCol, true);

        target.transform.SetParent(carryPoint, false);
        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;

        OnPlayerGrabbed?.Invoke();
    }

    private void ThrowPlayer()
    {
        GameObject playerToThrow = carriedPlayer;

        Rigidbody rbToThrow = carriedRb;
        Collider colToThrow = carriedCol;

        PlayerMovement movementToRestore = carriedMovement;
        PlayerControllerMovement controllerMovementToRestore = carriedControllerMovement;

        playerToThrow.transform.SetParent(null, true);

        rbToThrow.isKinematic = false;
        rbToThrow.linearVelocity = Vector3.zero;
        rbToThrow.angularVelocity = Vector3.zero;

        if (carrierCol != null && colToThrow != null)
            Physics.IgnoreCollision(carrierCol, colToThrow, false);

        rbToThrow.linearVelocity = GetThrowVelocity();

        if (landingIndicator != null)
            landingIndicator.SetActive(false);

        OnPlayerThrown?.Invoke();

        ResetCarryReferences();

        StartCoroutine(ReEnableMovementAfterThrow(
            movementToRestore,
            controllerMovementToRestore,
            movementDisableDuration
        ));
    }

    private Vector3 GetThrowVelocity()
    {
        Vector3 flatDirection = lastDirection;
        flatDirection.y = 0f;

        if (flatDirection.sqrMagnitude < 0.01f)
            flatDirection = transform.forward;

        flatDirection.Normalize();

        Vector3 velocity = flatDirection * throwForwardForce;
        velocity.y = throwUpForce;

        return velocity;
    }

    private void UpdateLandingPreview()
    {
        if (landingIndicator == null)
            return;

        if (carriedPlayer == null)
        {
            landingIndicator.SetActive(false);
            return;
        }

        Vector3 velocity = GetThrowVelocity();

        Vector3 flatVelocity = new Vector3(
            velocity.x,
            0f,
            velocity.z
        );

        Vector3 predictedPosition =
            carryPoint.position +
            flatVelocity * landingPreviewDistanceMultiplier;

        Vector3 rayStart =
            predictedPosition +
            Vector3.up * raycastStartHeight;

        if (Physics.Raycast(
            rayStart,
            Vector3.down,
            out RaycastHit hit,
            raycastDistance,
            landingGroundLayer))
        {
            landingIndicator.transform.position =
                hit.point +
                Vector3.up * landingPreviewHeightOffset;

            landingIndicator.transform.rotation = Quaternion.identity;
            landingIndicator.SetActive(true);
        }
        else
        {
            landingIndicator.SetActive(false);
        }
    }

    private IEnumerator ReEnableMovementAfterThrow(
        PlayerMovement movement,
        PlayerControllerMovement controllerMovement,
        float delay
    )
    {
        yield return new WaitForSeconds(delay);

        if (movement != null)
            movement.enabled = true;

        if (controllerMovement != null)
            controllerMovement.enabled = true;
    }

    private void ResetCarryReferences()
    {
        carriedPlayer = null;
        carriedRb = null;
        carriedCol = null;

        carriedMovement = null;
        carriedControllerMovement = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, grabRange);
    }
}
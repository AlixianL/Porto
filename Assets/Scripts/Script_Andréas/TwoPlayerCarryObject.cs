using UnityEngine;
using UnityEngine.Events;

public class TwoPlayerCarryObject : MonoBehaviour
{
    private enum CarryState
    {
        Free,
        WaitingSecondPlayer,
        CarriedByTwo
    }

    [Header("Carry")]
    [SerializeField] private float carryHeight = 0.8f;
    [SerializeField] private float moveSmoothness = 12f;
    [SerializeField] private float rotationSmoothness = 12f;

    [Header("Distance Limits")]
    [SerializeField] private float maxDistanceFromObjectWhenWaiting = 2f;
    [SerializeField] private float maxDistanceBetweenPlayers = 3f;

    [Header("Throw")]
    [SerializeField] private float throwForwardForce = 6f;
    [SerializeField] private float throwUpForce = 4.5f;

    [Header("Landing Preview")]
    [SerializeField] private GameObject landingIndicator;
    [SerializeField] private LayerMask landingGroundLayer;
    [SerializeField] private float landingPreviewDistanceMultiplier = 1f;
    [SerializeField] private float landingPreviewHeightOffset = 0.02f;
    [SerializeField] private float raycastStartHeight = 20f;
    [SerializeField] private float raycastDistance = 50f;

    [Header("Events")]
    public UnityEvent OnFirstPlayerRegistered;
    public UnityEvent OnCarryStarted;
    public UnityEvent OnCarryReleased;
    public UnityEvent OnHeavyObjectThrown;

    [Header("Debug")]
    [SerializeField] private bool showDebug;

    private CarryState state = CarryState.Free;

    private TwoPlayerCarryInteractor playerA;
    private TwoPlayerCarryInteractor playerB;

    private Rigidbody rb;

    private bool IsFullyCarried => playerA != null && playerB != null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (landingIndicator != null)
            landingIndicator.SetActive(false);
    }

    private void FixedUpdate()
    {
        CheckDistanceLimits();

        if (state == CarryState.CarriedByTwo && IsFullyCarried)
            MoveBetweenPlayers();
    }

    private void LateUpdate()
    {
        UpdateLandingPreview();
    }

    public bool TryRegisterPlayer(TwoPlayerCarryInteractor player)
    {
        if (player == null)
            return false;

        if (state == CarryState.Free)
        {
            playerA = player;
            state = CarryState.WaitingSecondPlayer;

            OnFirstPlayerRegistered?.Invoke();

            if (showDebug)
                Debug.Log(name + " : premier joueur accroché, attente du deuxième.");

            return true;
        }

        if (state == CarryState.WaitingSecondPlayer)
        {
            if (player == playerA)
                return true;

            playerB = player;
            StartTwoPlayerCarry();

            return true;
        }

        if (state == CarryState.CarriedByTwo)
            return player == playerA || player == playerB;

        return false;
    }

    public void RequestAction(TwoPlayerCarryInteractor requester)
    {
        if (requester == null)
            return;

        if (state == CarryState.WaitingSecondPlayer)
        {
            ReleasePlayer(requester);
            return;
        }

        if (state == CarryState.CarriedByTwo)
        {
            Throw(requester.GetInputDirection());
        }
    }

    public void ReleasePlayer(TwoPlayerCarryInteractor player)
    {
        if (player == null)
            return;

        if (playerA == player)
            playerA = null;

        if (playerB == player)
            playerB = null;

        player.ClearHeavyObject(this);

        if (state == CarryState.CarriedByTwo)
            StopPhysicsCarry();

        if (playerA == null && playerB == null)
        {
            state = CarryState.Free;
        }
        else
        {
            if (playerA == null)
            {
                playerA = playerB;
                playerB = null;
            }

            state = CarryState.WaitingSecondPlayer;
        }

        HideLandingPreview();
        OnCarryReleased?.Invoke();

        if (showDebug)
            Debug.Log(name + " : joueur relâché.");
    }

    public void ForceReleaseAll()
    {
        if (playerA != null)
            playerA.ClearHeavyObject(this);

        if (playerB != null)
            playerB.ClearHeavyObject(this);

        playerA = null;
        playerB = null;

        StopPhysicsCarry();

        state = CarryState.Free;

        HideLandingPreview();
        OnCarryReleased?.Invoke();

        if (showDebug)
            Debug.Log(name + " : release all.");
    }

    private void StartTwoPlayerCarry()
    {
        state = CarryState.CarriedByTwo;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        OnCarryStarted?.Invoke();

        if (showDebug)
            Debug.Log(name + " : portage à deux commencé.");
    }

    private void StopPhysicsCarry()
    {
        if (rb == null)
            return;

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void MoveBetweenPlayers()
    {
        Vector3 positionA = playerA.transform.position;
        Vector3 positionB = playerB.transform.position;

        Vector3 targetPosition = (positionA + positionB) * 0.5f;
        targetPosition.y += carryHeight;

        if (rb != null && rb.isKinematic)
        {
            Vector3 smoothedPosition = Vector3.Lerp(
                rb.position,
                targetPosition,
                moveSmoothness * Time.fixedDeltaTime
            );

            rb.MovePosition(smoothedPosition);
        }
        else
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                moveSmoothness * Time.fixedDeltaTime
            );
        }

        Vector3 directionBetweenPlayers = positionB - positionA;
        directionBetweenPlayers.y = 0f;

        if (directionBetweenPlayers.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(
                directionBetweenPlayers.normalized,
                Vector3.up
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSmoothness * Time.fixedDeltaTime
            );
        }
    }

    private void Throw(Vector3 direction)
    {
        if (!IsFullyCarried)
            return;

        ForceReleaseAll();

        if (rb == null)
            return;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            direction = transform.forward;

        direction.Normalize();

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 velocity = direction * throwForwardForce;
        velocity.y = throwUpForce;

        rb.linearVelocity = velocity;

        HideLandingPreview();
        OnHeavyObjectThrown?.Invoke();

        if (showDebug)
            Debug.Log(name + " : lancé avec velocity " + velocity);
    }

    private void CheckDistanceLimits()
    {
        if (state == CarryState.WaitingSecondPlayer)
        {
            if (playerA == null)
            {
                ForceReleaseAll();
                return;
            }

            float distance = Vector3.Distance(
                playerA.transform.position,
                transform.position
            );

            if (distance > maxDistanceFromObjectWhenWaiting)
                ForceReleaseAll();

            return;
        }

        if (state == CarryState.CarriedByTwo)
        {
            if (playerA == null || playerB == null)
            {
                ForceReleaseAll();
                return;
            }

            float distance = Vector3.Distance(
                playerA.transform.position,
                playerB.transform.position
            );

            if (distance > maxDistanceBetweenPlayers)
                ForceReleaseAll();
        }
    }

    private void UpdateLandingPreview()
    {
        if (landingIndicator == null)
            return;

        if (state != CarryState.CarriedByTwo || !IsFullyCarried)
        {
            HideLandingPreview();
            return;
        }

        Vector3 direction = GetAverageThrowDirection();
        Vector3 velocity = direction * throwForwardForce;
        velocity.y = throwUpForce;

        Vector3 flatVelocity = new Vector3(
            velocity.x,
            0f,
            velocity.z
        );

        Vector3 predictedPosition =
            transform.position +
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
                hit.point + Vector3.up * landingPreviewHeightOffset;

            landingIndicator.transform.rotation = Quaternion.identity;
            landingIndicator.SetActive(true);
        }
        else
        {
            HideLandingPreview();
        }
    }

    private Vector3 GetAverageThrowDirection()
    {
        Vector3 directionA = playerA != null
            ? playerA.GetInputDirection()
            : Vector3.zero;

        Vector3 directionB = playerB != null
            ? playerB.GetInputDirection()
            : Vector3.zero;

        Vector3 direction = directionA + directionB;

        if (direction.sqrMagnitude < 0.01f)
            direction = transform.forward;

        direction.y = 0f;
        return direction.normalized;
    }

    private void HideLandingPreview()
    {
        if (landingIndicator != null)
            landingIndicator.SetActive(false);
    }

    private void OnDisable()
    {
        if (!gameObject.scene.isLoaded)
            return;

        ForceReleaseAll();
    }
}
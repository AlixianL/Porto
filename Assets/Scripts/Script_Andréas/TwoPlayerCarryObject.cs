using UnityEngine;

public class TwoPlayerCarryObject : MonoBehaviour
{
    [Header("Carry")]
    [SerializeField] private float carryHeight = 0.8f;
    [SerializeField] private float moveSmoothness = 12f;

    [Header("Distance Limits")]
    [SerializeField] private float maxPlayerDistance = 3f;
    [SerializeField] private float maxSinglePlayerDistance = 2f;

    [Header("Throw")]
    [SerializeField] private float throwForwardForce = 6f;
    [SerializeField] private float throwUpForce = 4.5f;

    private TwoPlayerCarryInteractor playerA;
    private TwoPlayerCarryInteractor playerB;

    private Rigidbody rb;
    private Collider col;

    private bool IsFullyCarried => playerA != null && playerB != null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        CheckDistanceLimits();

        if (!IsFullyCarried)
            return;

        MoveBetweenPlayers();
    }

    public bool TryRegisterPlayer(TwoPlayerCarryInteractor player)
    {
        if (player == null)
            return false;

        if (playerA == player || playerB == player)
            return true;

        if (playerA == null)
        {
            playerA = player;
            return true;
        }

        if (playerB == null)
        {
            playerB = player;
            StartCarry();
            return true;
        }

        return false;
    }
    public void RequestAction(TwoPlayerCarryInteractor requester)
    {
        if (IsFullyCarried)
        {
            Throw(requester.GetInputDirection());
        }
        else
        {
            ReleasePlayer(requester);
        }
    }

    public void ReleasePlayer(TwoPlayerCarryInteractor player)
    {
        if (playerA == player)
            playerA = null;

        if (playerB == player)
            playerB = null;

        player.ClearHeavyObject(this);

        StopCarry();
    }

    public void ForceReleaseAll()
    {
        if (playerA != null)
            playerA.ClearHeavyObject(this);

        if (playerB != null)
            playerB.ClearHeavyObject(this);

        playerA = null;
        playerB = null;

        StopCarry();
    }

    private void StartCarry()
    {
        if (rb == null)
            return;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    private void StopCarry()
    {
        if (rb == null)
            return;

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void Throw(Vector3 direction)
    {
        ForceReleaseAll();

        if (rb == null)
            return;

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            direction = transform.forward;

        direction.Normalize();

        Vector3 velocity = direction * throwForwardForce;
        velocity.y = throwUpForce;

        rb.linearVelocity = velocity;
    }
    private void MoveBetweenPlayers()
    {
        Vector3 playerAPosition = playerA.transform.position;
        Vector3 playerBPosition = playerB.transform.position;

        Vector3 middlePosition = (playerAPosition + playerBPosition) * 0.5f;
        middlePosition.y += carryHeight;

        transform.position = Vector3.Lerp(
            transform.position,
            middlePosition,
            moveSmoothness * Time.fixedDeltaTime
        );

        Vector3 directionBetweenPlayers = playerBPosition - playerAPosition;
        directionBetweenPlayers.y = 0f;

        if (directionBetweenPlayers.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(directionBetweenPlayers.normalized, Vector3.up);
    }

    private void CheckDistanceLimits()
    {
        if (IsFullyCarried)
        {
            float distance = Vector3.Distance(playerA.transform.position, playerB.transform.position);

            if (distance > maxPlayerDistance)
                ForceReleaseAll();

            return;
        }

        if (playerA != null)
        {
            float distance = Vector3.Distance(playerA.transform.position, transform.position);

            if (distance > maxSinglePlayerDistance)
                ForceReleaseAll();
        }
    }

    private void OnDisable()
    {
        ForceReleaseAll();
    }
}
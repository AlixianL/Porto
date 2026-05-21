using System.Collections;
using UnityEngine;

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
    [SerializeField] private float throwForwardForce = 12f;
    [SerializeField] private float throwUpForce = 5f;
    [SerializeField] private float movementDisableDuration = 1f;
    [SerializeField] private float actionCooldown = 0.2f;

    private PlayerMovement carrierMovement;
    private PlayerControllerMovement carrierControllerMovement;
    private Collider carrierCol;

    private GameObject carriedPlayer;
    private Rigidbody carriedRb;
    private Collider carriedCol;

    private PlayerMovement carriedMovement;
    private PlayerControllerMovement carriedControllerMovement;

    private Vector3 lastDirection = Vector3.forward;
    
    private float nextActionTime;

    void Start()
    {
        carrierMovement = GetComponent<PlayerMovement>();
        carrierControllerMovement = GetComponent<PlayerControllerMovement>();
        carrierCol = GetComponent<Collider>();
    }

    void Update()
    {
        UpdateCarryDirection();
    }

    void LateUpdate()
    {
        UpdateCarryPointPosition();

        if (carriedPlayer != null)
        {
            carriedPlayer.transform.position = carryPoint.position;
            carriedPlayer.transform.rotation = carryPoint.rotation;
        }
    }

    public void TryCarryAction()
    {
        if (Time.time < nextActionTime)
            return;

        nextActionTime = Time.time + actionCooldown;

        if (carriedPlayer == null)
            TryGrabPlayer();
        else
            ThrowPlayer();
    }

    void UpdateCarryDirection()
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

    void UpdateCarryPointPosition()
    {
        if (carryPoint == null)
            return;

        carryPoint.position = transform.position + lastDirection * carryDistance + Vector3.up * carryHeight;
        carryPoint.rotation = Quaternion.LookRotation(lastDirection, Vector3.up);
    }

    void TryGrabPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, grabRange, playerLayer);

        foreach (Collider hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            GrabPlayer(hit.gameObject);
            return;
        }

        Debug.Log(gameObject.name + " : aucun joueur à attraper");
    }

    void GrabPlayer(GameObject target)
    {
        TutorialManager.Instance?.ValidateGrabPlayer();
        carriedPlayer = target;
        carriedRb = target.GetComponent<Rigidbody>();
        carriedCol = target.GetComponent<Collider>();

        carriedMovement = target.GetComponent<PlayerMovement>();
        carriedControllerMovement = target.GetComponent<PlayerControllerMovement>();

        if (carriedRb == null)
        {
            Debug.LogError("Le joueur porté n'a pas de Rigidbody.");
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

        Debug.Log(gameObject.name + " porte " + target.name);
    }

    void ThrowPlayer()
    {
        TutorialManager.Instance?.ValidateThrowPlayer();
        GameObject playerToThrow = carriedPlayer;
        Rigidbody rbToThrow = carriedRb;
        Collider colToThrow = carriedCol;

        PlayerMovement movementToRestore = carriedMovement;
        PlayerControllerMovement controllerMovementToRestore = carriedControllerMovement;

        Vector3 throwDirection = lastDirection;
        throwDirection.y = 0f;

        if (throwDirection.sqrMagnitude < 0.01f)
            throwDirection = transform.forward;

        throwDirection.Normalize();

        playerToThrow.transform.SetParent(null, true);

        rbToThrow.isKinematic = false;
        rbToThrow.linearVelocity = Vector3.zero;
        rbToThrow.angularVelocity = Vector3.zero;

        if (carrierCol != null && colToThrow != null)
            Physics.IgnoreCollision(carrierCol, colToThrow, false);

        Vector3 finalVelocity = new Vector3(
            throwDirection.x * throwForwardForce,
            throwUpForce,
            throwDirection.z * throwForwardForce
        );

        rbToThrow.linearVelocity = finalVelocity;

        Debug.Log(gameObject.name + " lance " + playerToThrow.name + " avec velocity : " + finalVelocity);

        ResetCarryReferences();

        StartCoroutine(ReEnableMovementAfterThrow(
            movementToRestore,
            controllerMovementToRestore,
            movementDisableDuration
        ));
    }

    IEnumerator ReEnableMovementAfterThrow(
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

    void ResetCarryReferences()
    {
        carriedPlayer = null;
        carriedRb = null;
        carriedCol = null;
        carriedMovement = null;
        carriedControllerMovement = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, grabRange);
    }
}
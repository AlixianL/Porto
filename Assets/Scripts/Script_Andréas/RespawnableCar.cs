using System.Collections;
using UnityEngine;

public class RespawnableCar : MonoBehaviour
{
    [SerializeField] private float deathY = -5f;
    [SerializeField] private float respawnFreezeDuration = 0.25f;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private Rigidbody rb;
    private CarMovement carMovement;

    private bool isRespawning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        carMovement = GetComponent<CarMovement>();

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void Update()
    {
        if (!isRespawning && transform.position.y <= deathY)
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        isRespawning = true;

        if (carMovement != null)
        {
            carMovement.ResetCarMovement();
            carMovement.enabled = false;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;

        transform.position = startPosition;
        transform.rotation = startRotation;

        yield return new WaitForSeconds(respawnFreezeDuration);

        rb.isKinematic = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (carMovement != null)
        {
            carMovement.ResetCarMovement();
            carMovement.enabled = true;
        }

        isRespawning = false;
    }
}
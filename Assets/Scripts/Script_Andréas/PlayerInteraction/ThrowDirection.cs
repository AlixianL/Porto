using UnityEngine;

public class ThrowDirection : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform throwPoint;

    [Header("Throw Point")]
    [SerializeField] private float throwPointDistance = 1f;
    [SerializeField] private float throwPointHeight = 1f;

    private Vector3 lastPosition;
    private Vector3 lastDirection = Vector3.forward;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 movementDelta = transform.position - lastPosition;
        movementDelta.y = 0f;

        if (movementDelta.sqrMagnitude > 0.0001f)
        {
            lastDirection = movementDelta.normalized;
        }

        throwPoint.position = transform.position + lastDirection * throwPointDistance + Vector3.up * throwPointHeight;
        throwPoint.rotation = Quaternion.LookRotation(lastDirection, Vector3.up);

        lastPosition = transform.position;
    }

    public Vector3 GetDirection()
    {
        return lastDirection;
    }
}
using UnityEngine;

public class PlayerAimDirection : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform aimRoot;

    [Header("Input")]
    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string verticalAxis = "Vertical";

    private Vector3 lastAimDirection = Vector3.forward;

    void Update()
    {
        float horizontal = Input.GetAxisRaw(horizontalAxis);
        float vertical = Input.GetAxisRaw(verticalAxis);

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical);

        if (inputDirection.sqrMagnitude > 0.01f)
        {
            lastAimDirection = inputDirection.normalized;

            aimRoot.rotation = Quaternion.LookRotation(lastAimDirection, Vector3.up);
        }
    }

    public Vector3 GetAimDirection()
    {
        return lastAimDirection;
    }
}
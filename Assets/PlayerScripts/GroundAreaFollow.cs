using UnityEngine;

[DisallowMultipleComponent]
public class GroundAreaFollow : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Boundaries")]
    [SerializeField] private bool limitX = false;
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;
    [SerializeField] private bool limitZ = false;
    [SerializeField] private float minZ = -5f;
    [SerializeField] private float maxZ = 5f;

    public void ApplyMovementInput(Vector3 movementInput, float deltaTime)
    {
        Vector3 movement = movementInput;

        if (movement.sqrMagnitude > 1f)
        {
            movement.Normalize();
        }

        Vector3 nextPosition = transform.position +
            new Vector3(movement.x, 0f, movement.z) * moveSpeed * deltaTime;

        if (limitX)
        {
            nextPosition.x = Mathf.Clamp(nextPosition.x, minX, maxX);
        }

        if (limitZ)
        {
            nextPosition.z = Mathf.Clamp(nextPosition.z, minZ, maxZ);
        }

        transform.position = nextPosition;
    }
}

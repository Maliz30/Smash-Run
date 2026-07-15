using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PhysicsMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float maxVelocity = 7f;
    [SerializeField] private float counterFriction = 0.15f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundMask = -1;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 3200f;
    [SerializeField] private float dashDuration = 0.15f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip dashSound;

    private Rigidbody rb;
    private Collider col;
    private PlayerInputHandler inputHandler;
    private PlayerDashCharges dashCharges;

    private float dashTimeRemaining;
    private Vector3 dashDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        inputHandler = GetComponent<PlayerInputHandler>();
        dashCharges = GetComponent<PlayerDashCharges>();

        rb.constraints = RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        if (inputHandler.ConsumeJump())
        {
            TryJump();
        }

        if (inputHandler.ConsumeDash())
        {
            TryDash();
        }

        if (dashTimeRemaining > 0f)
        {
            ApplyDashVelocity();
            dashTimeRemaining -= Time.fixedDeltaTime;
        }
        else
        {
            MoveCube();
            ApplyCustomFriction();
        }
    }

    private void MoveCube()
    {
        Vector2 input = inputHandler.MovementInput;
        GetComponentInChildren<Animator>().SetBool("isMoving", input.sqrMagnitude > 0.01f);
        Vector3 moveDirection = new Vector3(input.x, 0f, input.y);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            moveDirection.Normalize();
            rb.AddForce(moveDirection * moveSpeed, ForceMode.Acceleration);
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 15f);
            
        }

        
        Vector3 horizontalVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        if (horizontalVelocity.magnitude > maxVelocity)
        {
            Vector3 clampedVelocity =
                horizontalVelocity.normalized * maxVelocity;

            rb.linearVelocity = new Vector3(
                clampedVelocity.x,
                rb.linearVelocity.y,
                clampedVelocity.z
            );
        }
    }

    private void ApplyCustomFriction()
    {
        if (!IsGrounded())
        {
            return;
        }

        if (inputHandler.MovementInput.sqrMagnitude < 0.01f)
        {
            Vector3 currentVel = rb.linearVelocity;
            currentVel.x *= 1f - counterFriction;
            currentVel.z *= 1f - counterFriction;

            rb.linearVelocity = new Vector3(
                currentVel.x,
                rb.linearVelocity.y,
                currentVel.z
            );
        }
        
    }

    private void TryJump()
    {
        if (!IsGrounded())
        {
            return;
        }

        Vector3 velocity = rb.linearVelocity;

        if (velocity.y < 0f)
        {
            velocity.y = 0f;
        }

        rb.linearVelocity = velocity;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }

    private void TryDash()
    {
        if (dashCharges == null || !dashCharges.TryConsumeCharge())
        {
            return;
        }

        if (audioSource != null && dashSound != null) audioSource.PlayOneShot(dashSound);
        
        dashDirection = GetDashDirection();
        dashTimeRemaining = dashDuration;
    }

    private void ApplyDashVelocity()
    {
        rb.linearVelocity = new Vector3(
            dashDirection.x * dashSpeed,
            rb.linearVelocity.y,
            dashDirection.z * dashSpeed
        );
    }

    private Vector3 GetDashDirection()
    {
        Vector2 input = inputHandler.MovementInput;
        Vector3 inputDirection = new Vector3(input.x, 0f, input.y);

        if (inputDirection.sqrMagnitude > 0.01f)
        {
            return inputDirection.normalized;
        }

        Vector3 horizontalVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        if (horizontalVelocity.sqrMagnitude > 0.01f)
        {
            return horizontalVelocity.normalized;
        }

        return transform.forward;
    }

    private bool IsGrounded()
    {
        Vector3 origin = col.bounds.center;
        float rayLength = col.bounds.extents.y + groundCheckDistance;

        return Physics.Raycast(
            origin,
            Vector3.down,
            rayLength,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }
}
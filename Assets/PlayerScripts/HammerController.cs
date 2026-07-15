using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class HammerController : MonoBehaviour
{
    private enum HammerState
    {
        Idle,
        WindingUp,
        Attacking,
        Recovering,
    }

    [Header("Input")]
    [SerializeField] private MonoBehaviour inputProviderSource;
    [SerializeField] private bool autoFindProvider = true;

    [Header("Shared Attack Area")]
    [SerializeField] private GroundAreaFollow attackArea;
    [SerializeField] private HammerAttackDispatcher attackDispatcher;

    [Header("Movement")]
    [SerializeField] private float minHeight = 1.5f;
    [SerializeField] private float maxHeight = 8f;
    [SerializeField] private float defaultHeight = 4f;

    [Header("Swing Rotation")]
    [SerializeField] private Vector3 headLocalDirection = Vector3.forward;
    [Tooltip("How many degrees to tilt the head back during wind-up.")]
    [SerializeField] private float windUpTiltAngle = 30f;

    [Header("Attack")]
    [SerializeField] private float windUpHeight = 2f;
    [SerializeField] private float windUpDuration = 0.5f;
    [SerializeField] private float attackSpeed = 20f;
    [SerializeField] private float attackMaxDuration = 1.5f;
    [SerializeField] private float recoverDuration = 0.8f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float groundY = 0f;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private LayerMask damageLayer = -1;

    [Header("Audio")]
    [SerializeField] private AudioSource hammerAudioSource;
    [SerializeField] private AudioClip hitSound;

    private Rigidbody rb;
    private readonly List<IHammerInputProvider> inputProviders = new();
    private readonly Dictionary<Object, Vector3> buttonMovementInputs = new();
    private Quaternion initialRotation;
    private Quaternion idleRotationTarget;
    private Quaternion windUpRotation;
    private Quaternion attackRotation;
    private bool hasAppliedDamageThisAttack;
    private float stateTimer;
    private float attackCooldownTimer;
    private Vector3 homePosition;
    private Vector3 windUpStartPosition;
    private Vector3 attackTargetPosition;
    private Vector3 recoverStartPosition;
    private Vector3 recoverTargetPosition;
    private Quaternion recoverStartRotation;
    private Quaternion recoverTargetRotation;
    private HammerState currentState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        initialRotation = transform.rotation;
        idleRotationTarget = initialRotation;

        rb.useGravity = false;
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        ResolveInputProvider();

        Vector3 startPosition = transform.position;
        startPosition.y = Mathf.Clamp(defaultHeight, minHeight, maxHeight);
        homePosition = startPosition;
        MoveToPosition(startPosition);
        rb.MoveRotation(initialRotation);
        currentState = HammerState.Idle;

        ComputeSwingRotations();
    }

    private void ComputeSwingRotations()
    {
        Vector3 headLocal = headLocalDirection.normalized;

        if (headLocal == Vector3.zero)
        {
            headLocal = Vector3.forward;
        }

        Vector3 headWorld = (initialRotation * headLocal).normalized;
        Vector3 swingAxis = Vector3.Cross(headWorld, Vector3.down).normalized;

        if (swingAxis.sqrMagnitude < 0.001f)
        {
            swingAxis = Vector3.right;
        }

        attackRotation = Quaternion.FromToRotation(headWorld, Vector3.down) * initialRotation;
        windUpRotation = Quaternion.AngleAxis(-windUpTiltAngle, swingAxis) * initialRotation;
    }

    private void Update()
    {
        Vector3 providerMovementInput = Vector3.zero;
        bool attackTriggered = false;

        foreach (IHammerInputProvider inputProvider in inputProviders)
        {
            providerMovementInput += inputProvider.GetMovementInput();

            if (inputProvider.GetAttackTrigger())
            {
                attackTriggered = true;
            }
        }

        if (providerMovementInput.sqrMagnitude > 1f)
        {
            providerMovementInput.Normalize();
        }

        foreach (Vector3 buttonMovement in buttonMovementInputs.Values)
        {
            providerMovementInput += buttonMovement;
        }

        if (providerMovementInput.sqrMagnitude > 1f)
        {
            providerMovementInput.Normalize();
        }

        if (attackArea != null)
        {
            attackArea.ApplyMovementInput(providerMovementInput, Time.deltaTime);
        }

        if (attackTriggered && attackDispatcher != null)
        {
            attackDispatcher.TriggerAttack();
        }
    }

    private void FixedUpdate()
    {
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.fixedDeltaTime;
        }

        switch (currentState)
        {
            case HammerState.Idle:
                UpdateIdleRotation();
                break;
            case HammerState.WindingUp:
                UpdateWindingUpRotation();
                UpdateWindingUp();
                break;
            case HammerState.Attacking:
                UpdateAttackingRotation();
                UpdateAttacking();
                break;
            case HammerState.Recovering:
                UpdateRecoveringRotation();
                UpdateRecovering();
                break;
        }
    }

    public void SetInputProvider(MonoBehaviour providerSource)
    {
        inputProviderSource = providerSource;
        ResolveInputProvider();
    }

    public bool TryStartAttack(Vector3 targetPosition)
    {
        if (currentState != HammerState.Idle || attackCooldownTimer > 0f)
        {
            return false;
        }

        attackTargetPosition = new Vector3(targetPosition.x, homePosition.y, targetPosition.z);
        TransitionTo(HammerState.WindingUp);
        return true;
    }

    public void SetButtonMovement(Object source, Vector3 movementInput)
    {
        if (source == null)
        {
            return;
        }

        buttonMovementInputs[source] = movementInput;
    }

    /// <summary>
    /// Registra um IHammerInputProvider externo (ex: botão da cabine).
    /// Pode ser chamado a qualquer momento, inclusive depois do Start.
    /// </summary>
    public void RegisterInputProvider(IHammerInputProvider provider)
    {
        if (provider == null)
            return;

        if (!inputProviders.Contains(provider))
        {
            inputProviders.Add(provider);
        }
    }

    private void UpdateIdleRotation()
    {
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, idleRotationTarget, 0.15f));
    }

    private void UpdateWindingUpRotation()
    {
        float duration = Mathf.Max(windUpDuration, 0.01f);
        float t = Mathf.Clamp01(stateTimer / duration);
        rb.MoveRotation(Quaternion.Slerp(initialRotation, windUpRotation, t));
    }

    private void UpdateAttackingRotation()
    {
        float duration = Mathf.Max(attackMaxDuration, 0.01f);
        float t = Mathf.Clamp01(stateTimer / duration);
        rb.MoveRotation(Quaternion.Slerp(windUpRotation, attackRotation, t));
    }

    private void UpdateRecoveringRotation()
    {
        float duration = Mathf.Max(recoverDuration, 0.01f);
        float t = Mathf.Clamp01(stateTimer / duration);
        float easedT = 1f - Mathf.Pow(1f - t, 3f);
        rb.MoveRotation(Quaternion.Slerp(recoverStartRotation, recoverTargetRotation, easedT));
    }

    private void UpdateWindingUp()
    {
        stateTimer += Time.fixedDeltaTime;

        float duration = Mathf.Max(windUpDuration, 0.01f);
        float t = Mathf.Clamp01(stateTimer / duration);

        Vector3 nextPosition = Vector3.Lerp(windUpStartPosition, attackTargetPosition, t);
        nextPosition.y = Mathf.Lerp(
            windUpStartPosition.y,
            Mathf.Min(maxHeight, windUpStartPosition.y + windUpHeight),
            t
        );

        MoveToPosition(nextPosition);

        if (stateTimer >= duration)
        {
            TransitionTo(HammerState.Attacking);
        }
    }

    private void UpdateAttacking()
    {
        stateTimer += Time.fixedDeltaTime;

        Vector3 nextPosition = rb.position + Vector3.down * attackSpeed * Time.fixedDeltaTime;

        if (nextPosition.y < groundY)
        {
            nextPosition.y = groundY;
        }

        MoveToPosition(nextPosition);

        if (Mathf.Approximately(nextPosition.y, groundY) || stateTimer >= attackMaxDuration)
        {
            TransitionTo(HammerState.Recovering);
        }
    }

    private void UpdateRecovering()
    {
        stateTimer += Time.fixedDeltaTime;

        float duration = Mathf.Max(recoverDuration, 0.01f);
        float t = Mathf.Clamp01(stateTimer / duration);
        float easedT = 1f - Mathf.Pow(1f - t, 3f);
        Vector3 nextPosition = Vector3.Lerp(recoverStartPosition, recoverTargetPosition, easedT);

        MoveToPosition(nextPosition);

        if (stateTimer >= duration)
        {
            TransitionTo(HammerState.Idle);
        }
    }

    private void TransitionTo(HammerState newState)
    {
        currentState = newState;
        stateTimer = 0f;

        switch (newState)
        {
            case HammerState.WindingUp:
                windUpStartPosition = homePosition;
                hasAppliedDamageThisAttack = false;
                break;
            case HammerState.Attacking:
                hasAppliedDamageThisAttack = false;
                break;
            case HammerState.Recovering:
                recoverStartPosition = rb.position;
                recoverStartRotation = rb.rotation;
                recoverTargetPosition = homePosition;
                recoverTargetPosition.y = Mathf.Clamp(homePosition.y, minHeight, maxHeight);
                recoverTargetRotation = idleRotationTarget;
                break;
            case HammerState.Idle:
                attackCooldownTimer = attackCooldown;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryApplyDamage(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryApplyDamage(collision.gameObject);
    }

    private void TryApplyDamage(GameObject other)
    {
        if (currentState != HammerState.Attacking || hasAppliedDamageThisAttack)
        {
            return;
        }

        if ((damageLayer.value & (1 << other.layer)) == 0)
        {
            return;
        }
        Debug.Log("Player entrou na área do martelo!");
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            if (hammerAudioSource != null && hitSound != null) 
                hammerAudioSource.PlayOneShot(hitSound);
            playerHealth.TakeDamage(1); // Manda o sinal para o Player tirar vida e tocar o áudio
        }
        // TODO: apply damage via PlayerHealth when that system is ready.
        hasAppliedDamageThisAttack = true;
        TransitionTo(HammerState.Recovering);
    }

    private void ResolveInputProvider()
    {
        inputProviders.Clear();

        if (inputProviderSource is IHammerInputProvider explicitProvider)
        {
            inputProviders.Add(explicitProvider);
        }

        if (inputProviders.Count == 0 && autoFindProvider)
        {
            MonoBehaviour[] behaviours = GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IHammerInputProvider provider)
                {
                    inputProviders.Add(provider);
                }
            }
        }

        if (inputProviders.Count == 0)
        {
            Debug.LogError("HammerController requires a MonoBehaviour that implements IHammerInputProvider.", this);
            enabled = false;
        }
    }

    private void MoveToPosition(Vector3 position)
    {
        rb.MovePosition(position);
    }
}

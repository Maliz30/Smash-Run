using UnityEngine;
using UnityEngine.UI; 

[DisallowMultipleComponent]
public class PressButton : MonoBehaviour
{

    [SerializeField] private HammerAttackDispatcher attackDispatcher;
    [SerializeField] private Transform plunger;
    [SerializeField] private bool autoFindDispatcher = true;
    [SerializeField] private bool autoCaptureRestPos = true;

    [SerializeField] private Image barraDeCooldown;

    [SerializeField] private float pressThreshold = 0.15f;
    [SerializeField] private float cooldown = 4.5f; 
    [SerializeField] private Vector3 plungerRestLocalPos = Vector3.zero;

    private float cooldownTimer;
    private bool wasPressedLastFrame;

    private void Start()
    {
        if (autoFindDispatcher && attackDispatcher == null)
        {
            attackDispatcher = FindAnyObjectByType<HammerAttackDispatcher>();
        }

        if (autoCaptureRestPos && plunger != null)
        {
            plungerRestLocalPos = plunger.localPosition;
        }
    }

    private void Update()
    {
        if (plunger == null || attackDispatcher == null)
        {
            return;
        }

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            
            if (barraDeCooldown != null)
            {
                barraDeCooldown.fillAmount = 1f - (cooldownTimer / cooldown);
            }
        }
        else
        {
            if (barraDeCooldown != null)
            {
                barraDeCooldown.fillAmount = 1f;
            }
        }

        Vector3 restWorldPosition = GetRestWorldPosition();
        float displacement = Vector3.Distance(plunger.position, restWorldPosition);
        bool isPressed = displacement >= pressThreshold;

        if (isPressed && !wasPressedLastFrame && cooldownTimer <= 0f)
        {
            if (attackDispatcher.TriggerAttack())
            {
                cooldownTimer = cooldown;
                if (barraDeCooldown != null) barraDeCooldown.fillAmount = 0f;
            }
        }

        wasPressedLastFrame = isPressed;
    }

    private void OnValidate()
    {
        pressThreshold = Mathf.Max(0.001f, pressThreshold);
        cooldown = Mathf.Max(0f, cooldown);

        if (plunger == null) return;

        MockPoseDriver mockPoseDriver = plunger.GetComponent<MockPoseDriver>();
        if (mockPoseDriver != null)
        {
            float mockDepth = mockPoseDriver.PressDepth;
            if (mockDepth > 0f && pressThreshold >= mockDepth)
            {
                Debug.LogWarning("PressButton pressThreshold should be smaller than MockPoseDriver pressDepth.", this);
            }
        }
    }

    private Vector3 GetRestWorldPosition()
    {
        Transform referenceSpace = plunger.parent;
        if (referenceSpace == null) return plungerRestLocalPos;
        return referenceSpace.TransformPoint(plungerRestLocalPos);
    }
}
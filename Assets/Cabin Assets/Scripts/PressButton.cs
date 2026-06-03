using UnityEngine;

[DisallowMultipleComponent]
public class PressButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HammerController hammerController;
    [SerializeField] private Transform plunger;
    [SerializeField] private bool autoFindHammer = true;
    [SerializeField] private bool autoCaptureRestPos = true;

    [Header("Press")]
    [SerializeField] private float pressThreshold = 0.15f;
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private Vector3 plungerRestLocalPos = Vector3.zero;

    private float cooldownTimer;
    private bool wasPressedLastFrame;

    private void Start()
    {
        if (autoFindHammer && hammerController == null)
        {
            hammerController = FindAnyObjectByType<HammerController>();
        }

        if (autoCaptureRestPos && plunger != null)
        {
            plungerRestLocalPos = plunger.localPosition;
        }
    }

    private void Update()
    {
        if (plunger == null || hammerController == null)
        {
            return;
        }

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        Vector3 restWorldPosition = GetRestWorldPosition();
        float displacement = Vector3.Distance(plunger.position, restWorldPosition);
        bool isPressed = displacement >= pressThreshold;

        if (isPressed && !wasPressedLastFrame && cooldownTimer <= 0f)
        {
            if (hammerController.TryStartAttack())
            {
                cooldownTimer = cooldown;
            }
        }

        wasPressedLastFrame = isPressed;
    }

    private void OnValidate()
    {
        pressThreshold = Mathf.Max(0.001f, pressThreshold);
        cooldown = Mathf.Max(0f, cooldown);

        if (plunger == null)
        {
            return;
        }

        MockPoseDriver mockPoseDriver = plunger.GetComponent<MockPoseDriver>();

        if (mockPoseDriver != null)
        {
            // Keep the threshold below the plunger travel so the button can actually fire.
            float mockDepth = mockPoseDriver.PressDepth;

            if (mockDepth > 0f && pressThreshold >= mockDepth)
            {
                Debug.LogWarning(
                    "PressButton pressThreshold should be smaller than MockPoseDriver pressDepth.",
                    this
                );
            }
        }
    }

    private Vector3 GetRestWorldPosition()
    {
        Transform referenceSpace = plunger.parent;

        if (referenceSpace == null)
        {
            return plungerRestLocalPos;
        }

        return referenceSpace.TransformPoint(plungerRestLocalPos);
    }
}
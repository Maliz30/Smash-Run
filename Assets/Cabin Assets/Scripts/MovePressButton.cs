using UnityEngine;

[DisallowMultipleComponent]
public class MovePressButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HammerController hammerController;
    [SerializeField] private Transform plunger;
    [SerializeField] private bool autoFindHammer = true;
    [SerializeField] private bool autoCaptureRestPos = true;

    [Header("Direction")]
    [SerializeField] private Vector3 moveDirection = Vector3.back;

    [Header("Press")]
    [SerializeField] private float pressThreshold = 0.15f;
    [SerializeField] private Vector3 plungerRestLocalPos = Vector3.zero;

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

        float displacement = Vector3.Distance(plunger.localPosition, plungerRestLocalPos);
        bool isPressed = displacement >= pressThreshold;

        hammerController.SetButtonMovement(this, isPressed ? moveDirection : Vector3.zero);
    }

    private void OnDisable()
    {
        if (hammerController != null)
        {
            hammerController.SetButtonMovement(this, Vector3.zero);
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

    private void OnValidate()
    {
        pressThreshold = Mathf.Max(0.001f, pressThreshold);

        if (plunger == null)
        {
            return;
        }

        MockPoseDriver mockPoseDriver = plunger.GetComponent<MockPoseDriver>();

        if (mockPoseDriver != null)
        {
            float mockDepth = mockPoseDriver.PressDepth;

            if (mockDepth > 0f && pressThreshold >= mockDepth)
            {
                Debug.LogWarning(
                    "MovePressButton pressThreshold should be smaller than MockPoseDriver pressDepth.",
                    this
                );
            }
        }
    }
}

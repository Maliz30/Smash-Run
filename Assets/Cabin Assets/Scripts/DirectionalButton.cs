using UnityEngine;

/// <summary>
/// A physical button that detects plunger displacement and exposes whether it is pressed.
/// Unlike PressButton (which triggers an attack), this simply reports its pressed state
/// so a ButtonHammerInput can combine multiple directional buttons into movement input.
/// 
/// Drag the hammer GameObject into hammerController, set the direction,
/// and assign the plunger Transform (the cylinder child).
/// </summary>
[DisallowMultipleComponent]
public class DirectionalButton : MonoBehaviour
{
    public enum HammerDirection
    {
        Forward,  // +Z
        Back,     // -Z
        Left,     // -X
        Right,    // +X
    }

    [Header("References")]
    [SerializeField] private Transform plunger;
    [SerializeField] private bool autoCaptureRestPos = true;

    [Header("Direction")]
    [SerializeField] private HammerDirection direction = HammerDirection.Forward;

    [Header("Press")]
    [SerializeField] private float pressThreshold = 0.15f;
    [SerializeField] private Vector3 plungerRestLocalPos = Vector3.zero;

    /// <summary>True while the plunger is displaced past the threshold.</summary>
    public bool IsPressed { get; private set; }

    /// <summary>World-space direction vector corresponding to the selected direction.</summary>
    public Vector3 DirectionVector { get; private set; }

    private void Awake()
    {
        DirectionVector = DirectionToVector(direction);
    }

    private void Start()
    {
        if (autoCaptureRestPos && plunger != null)
        {
            plungerRestLocalPos = plunger.localPosition;
        }
    }

    private void Update()
    {
        if (plunger == null)
        {
            IsPressed = false;
            return;
        }

        Vector3 restWorldPosition = GetRestWorldPosition();
        float displacement = Vector3.Distance(plunger.position, restWorldPosition);
        IsPressed = displacement >= pressThreshold;
    }

    private void OnValidate()
    {
        pressThreshold = Mathf.Max(0.001f, pressThreshold);
        DirectionVector = DirectionToVector(direction);
    }

    private Vector3 GetRestWorldPosition()
    {
        Transform referenceSpace = plunger.parent;
        if (referenceSpace == null)
            return plungerRestLocalPos;
        return referenceSpace.TransformPoint(plungerRestLocalPos);
    }

    private static Vector3 DirectionToVector(HammerDirection dir)
    {
        return dir switch
        {
            HammerDirection.Forward => Vector3.forward,
            HammerDirection.Back    => Vector3.back,
            HammerDirection.Left    => Vector3.left,
            HammerDirection.Right   => Vector3.right,
            _ => Vector3.forward,
        };
    }
}

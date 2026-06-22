using UnityEngine;

/// <summary>
/// IHammerInputProvider that reads from up to 4 DirectionalButton instances.
/// 
/// Place this on the same GameObject as the HammerController, then assign
/// the four directional buttons in the inspector. The HammerController will
/// auto-detect this as its input provider.
/// 
/// When multiple buttons are pressed simultaneously, the movement direction
/// is normalized so diagonal movement isn't faster.
/// </summary>
[DisallowMultipleComponent]
public class ButtonHammerInput : MonoBehaviour, IHammerInputProvider
{
    [Header("Directional Buttons")]
    [SerializeField] private DirectionalButton forwardButton;
    [SerializeField] private DirectionalButton backButton;
    [SerializeField] private DirectionalButton leftButton;
    [SerializeField] private DirectionalButton rightButton;

    [Header("Optional: Height Buttons")]
    [SerializeField] private DirectionalButton upButton;
    [SerializeField] private DirectionalButton downButton;

    public Vector3 GetMovementInput()
    {
        Vector3 input = Vector3.zero;

        if (forwardButton != null && forwardButton.IsPressed)
            input += forwardButton.DirectionVector;
        if (backButton != null && backButton.IsPressed)
            input += backButton.DirectionVector;
        if (leftButton != null && leftButton.IsPressed)
            input += leftButton.DirectionVector;
        if (rightButton != null && rightButton.IsPressed)
            input += rightButton.DirectionVector;

        // Normalize diagonal movement so speed is consistent
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        return input;
    }

    public float GetHeightInput()
    {
        float y = 0f;

        if (upButton != null && upButton.IsPressed)
            y += 1f;
        if (downButton != null && downButton.IsPressed)
            y -= 1f;

        return Mathf.Clamp(y, -1f, 1f);
    }

    public bool GetAttackTrigger()
    {
        // Directional buttons do not trigger attacks.
        // Reserve this for a separate dedicated attack button if needed.
        return false;
    }
}

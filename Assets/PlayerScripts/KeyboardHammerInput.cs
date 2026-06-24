using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class KeyboardHammerInput : MonoBehaviour, IHammerInputProvider
{
    [Header("Movement Keys")]
    [SerializeField] private Key forwardKey = Key.I;
    [SerializeField] private Key backKey = Key.K;
    [SerializeField] private Key leftKey = Key.J;
    [SerializeField] private Key rightKey = Key.L;

    [Header("Movement Buttons")]
    [SerializeField] private DirectionalButton forwardButton;
    [SerializeField] private DirectionalButton backButton;
    [SerializeField] private DirectionalButton leftButton;
    [SerializeField] private DirectionalButton rightButton;

    [Header("Height Keys")]
    [SerializeField] private Key upKey = Key.U;
    [SerializeField] private Key downKey = Key.O;

    [Header("Height Buttons")]
    [SerializeField] private DirectionalButton upButton;
    [SerializeField] private DirectionalButton downButton;

    [Header("Attack Keys")]
    [SerializeField] private Key attackKey = Key.P;

    public Vector3 GetMovementInput()
    {
        Keyboard keyboard = Keyboard.current;

        float x = 0f;
        float z = 0f;

        if (keyboard != null && keyboard[leftKey].isPressed)
        {
            x -= 1f;
        }

        if (keyboard != null && keyboard[rightKey].isPressed)
        {
            x += 1f;
        }

        if (keyboard != null && keyboard[forwardKey].isPressed)
        {
            z += 1f;
        }

        if (keyboard != null && keyboard[backKey].isPressed)
        {
            z -= 1f;
        }

        if (leftButton != null && leftButton.IsPressed)
        {
            x -= 1f;
        }

        if (rightButton != null && rightButton.IsPressed)
        {
            x += 1f;
        }

        if (forwardButton != null && forwardButton.IsPressed)
        {
            z += 1f;
        }

        if (backButton != null && backButton.IsPressed)
        {
            z -= 1f;
        }

        Vector3 movement = new Vector3(x, 0f, z);

        if (movement.sqrMagnitude > 1f)
        {
            movement.Normalize();
        }

        return movement;
    }

    public float GetHeightInput()
    {
        Keyboard keyboard = Keyboard.current;

        float y = 0f;

        if (keyboard != null && keyboard[upKey].isPressed)
        {
            y += 1f;
        }

        if (keyboard != null && keyboard[downKey].isPressed)
        {
            y -= 1f;
        }

        if (upButton != null && upButton.IsPressed)
        {
            y += 1f;
        }

        if (downButton != null && downButton.IsPressed)
        {
            y -= 1f;
        }

        return Mathf.Clamp(y, -1f, 1f);
    }

    public bool GetAttackTrigger()
    {
        Keyboard keyboard = Keyboard.current;
        return keyboard != null && keyboard[attackKey].wasPressedThisFrame;
    }
}
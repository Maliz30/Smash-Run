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

    [Header("Height Keys")]
    [SerializeField] private Key upKey = Key.U;
    [SerializeField] private Key downKey = Key.O;

    [Header("Attack Keys")]
    [SerializeField] private Key attackKey = Key.P;

    public Vector3 GetMovementInput()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return Vector3.zero;
        }

        float x = 0f;
        float z = 0f;

        if (keyboard[leftKey].isPressed)
        {
            x -= 1f;
        }

        if (keyboard[rightKey].isPressed)
        {
            x += 1f;
        }

        if (keyboard[forwardKey].isPressed)
        {
            z += 1f;
        }

        if (keyboard[backKey].isPressed)
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

        if (keyboard == null)
        {
            return 0f;
        }

        float y = 0f;

        if (keyboard[upKey].isPressed)
        {
            y += 1f;
        }

        if (keyboard[downKey].isPressed)
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
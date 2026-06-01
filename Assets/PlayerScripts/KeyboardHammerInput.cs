using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class KeyboardHammerInput : MonoBehaviour, IHammerInputProvider
{
    public Vector3 GetMovementInput()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return Vector3.zero;
        }

        float x = 0f;
        float z = 0f;

        if (keyboard.jKey.isPressed)
        {
            x -= 1f;
        }

        if (keyboard.lKey.isPressed)
        {
            x += 1f;
        }

        if (keyboard.iKey.isPressed)
        {
            z += 1f;
        }

        if (keyboard.kKey.isPressed)
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

        if (keyboard.uKey.isPressed)
        {
            y += 1f;
        }

        if (keyboard.oKey.isPressed)
        {
            y -= 1f;
        }

        return Mathf.Clamp(y, -1f, 1f);
    }

    public bool GetAttackTrigger()
    {
        Keyboard keyboard = Keyboard.current;
        return keyboard != null && keyboard.pKey.wasPressedThisFrame;
    }
}
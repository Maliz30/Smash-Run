using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MovementInput { get; private set; }

    private bool jumpQueued;
    private bool dashQueued;

    public void OnMove(InputValue value)
    {
        MovementInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.Get<float>() > 0.5f)
        {
            jumpQueued = true;
        }
    }

    public void OnDash(InputValue value)
    {
        if (value.Get<float>() > 0.5f)
        {
            dashQueued = true;
        }
    }

    public bool ConsumeJump()
    {
        if (!jumpQueued)
        {
            return false;
        }

        jumpQueued = false;
        return true;
    }

    public bool ConsumeDash()
    {
        if (!dashQueued)
        {
            return false;
        }

        dashQueued = false;
        return true;
    }
}
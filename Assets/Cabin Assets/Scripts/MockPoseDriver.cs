using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class MockPoseDriver : MonoBehaviour
{
    private enum PressAxis
    {
        LocalX,
        LocalY,
        LocalZ,
    }

    [Header("Motion")]
    [SerializeField] private float pressDepth = 0.3f;
    [SerializeField] private float pressSpeed = 5f;
    [SerializeField] private float returnSpeed = 3f;
    [SerializeField] private PressAxis pressAxis = PressAxis.LocalY;
    [SerializeField] private bool invertDirection = true;

    [Header("Input")]
    [SerializeField] private bool requireMouseOver = true;
    [SerializeField] private Key keyboardFallbackKey = Key.B;

    private Collider targetCollider;
    private Vector3 restLocalPosition;
    private Vector3 pressedLocalPosition;
    private bool isHeld;

    public float PressDepth => pressDepth;

    private void Awake()
    {
        targetCollider = GetComponent<Collider>();
        restLocalPosition = transform.localPosition;
        pressedLocalPosition = restLocalPosition + GetPressAxisVector() * pressDepth;
    }

    private void Update()
    {
        UpdateHeldState();

        float moveSpeed = isHeld ? pressSpeed : returnSpeed;
        Vector3 targetLocalPosition = isHeld ? pressedLocalPosition : restLocalPosition;

        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            targetLocalPosition,
            moveSpeed * Time.deltaTime
        );
    }

    private void OnValidate()
    {
        pressDepth = Mathf.Max(0.001f, pressDepth);
        pressSpeed = Mathf.Max(0.001f, pressSpeed);
        returnSpeed = Mathf.Max(0.001f, returnSpeed);
    }

    private void UpdateHeldState()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard[keyboardFallbackKey].isPressed)
            {
                isHeld = true;
                return;
            }

            if (keyboard[keyboardFallbackKey].wasReleasedThisFrame)
            {
                isHeld = false;
            }
        }

        Mouse mouse = Mouse.current;

        if (mouse == null)
        {
            return;
        }

        if (mouse.leftButton.wasPressedThisFrame)
        {
            bool mouseOverThis = !requireMouseOver || IsMouseOverThisCollider(mouse.position.ReadValue());

            if (mouseOverThis)
            {
                isHeld = true;
            }
        }

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            isHeld = false;
        }
    }

    private bool IsMouseOverThisCollider(Vector2 screenPosition)
    {
        if (targetCollider == null)
        {
            return false;
        }

        Camera targetCamera = Camera.main;

        if (targetCamera == null)
        {
            return false;
        }

        Ray ray = targetCamera.ScreenPointToRay(screenPosition);

        if (!targetCollider.Raycast(ray, out _, float.MaxValue))
        {
            return false;
        }

        return true;
    }

    private Vector3 GetPressAxisVector()
    {
        Vector3 axisVector = pressAxis switch
        {
            PressAxis.LocalX => Vector3.right,
            PressAxis.LocalY => Vector3.up,
            _ => Vector3.forward,
        };

        return invertDirection ? -axisVector : axisVector;
    }
}
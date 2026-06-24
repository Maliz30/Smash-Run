using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class VRPokeDriver : MonoBehaviour
{
    [Header("Motion")]
    [SerializeField] private float pressDepth = 0.3f;
    [SerializeField] private float pressSpeed = 5f;
    [SerializeField] private float returnSpeed = 3f;
    [SerializeField] private Vector3 pressDirection = Vector3.down;

    [Header("Collision")]
    [SerializeField] private string controllerTag = "Untagged"; // deixe Untagged, usa layer abaixo

    private Collider col;
    private Vector3 restLocalPosition;
    private Vector3 pressedLocalPosition;
    private bool isHeld;
    private float lastValidPressTime = float.NegativeInfinity;

    private void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
        restLocalPosition = transform.localPosition;
        pressedLocalPosition = restLocalPosition + pressDirection.normalized * pressDepth;
    }

    private void Update()
    {
        float releaseGraceTime = Mathf.Max(Time.fixedDeltaTime * 1.5f, 0.05f);
        isHeld = Time.time - lastValidPressTime <= releaseGraceTime;

        float speed = isHeld ? pressSpeed : returnSpeed;
        Vector3 target = isHeld ? pressedLocalPosition : restLocalPosition;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryMarkPress(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryMarkPress(other);
    }

    private void OnDisable()
    {
        isHeld = false;
        lastValidPressTime = float.NegativeInfinity;
    }

    private void TryMarkPress(Collider other)
    {
        if (other == null || other == col)
        {
            return;
        }

        if (other.transform.IsChildOf(transform.root))
        {
            return;
        }

        if (other.attachedRigidbody == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(controllerTag)
            && controllerTag != "Untagged"
            && !other.CompareTag(controllerTag))
        {
            return;
        }

        lastValidPressTime = Time.time;
    }
}

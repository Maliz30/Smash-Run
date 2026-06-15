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

    private void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
        restLocalPosition = transform.localPosition;
        pressedLocalPosition = restLocalPosition + pressDirection.normalized * pressDepth;
    }

    private void Update()
    {
        float speed = isHeld ? pressSpeed : returnSpeed;
        Vector3 target = isHeld ? pressedLocalPosition : restLocalPosition;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, target, speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detecta qualquer collider que seja do controle VR
        // (você pode filtrar por layer se preferir)
        isHeld = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isHeld = false;
    }
}

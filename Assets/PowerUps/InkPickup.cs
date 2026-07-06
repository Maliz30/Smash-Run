using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InkPickup : MonoBehaviour
{
    [SerializeField] private float blindDuration = 8f;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private VRVisionBlocker visionBlocker;
    [SerializeField] private bool autoFindVisionBlocker = true;

    private void Start()
    {
        if (autoFindVisionBlocker && visionBlocker == null)
        {
            visionBlocker = FindAnyObjectByType<VRVisionBlocker>();
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        PhysicsMovement player = other.GetComponent<PhysicsMovement>();

        if (player == null)
        {
            player = other.GetComponentInParent<PhysicsMovement>();
        }

        if (player == null || visionBlocker == null)
        {
            return;
        }

        visionBlocker.Block(blindDuration);
        gameObject.SetActive(false);
    }
}

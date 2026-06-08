using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DashPickup : MonoBehaviour
{
    [SerializeField] private int chargesToGive = 3;
    [SerializeField] private float rotationSpeed = 90f;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerDashCharges charges = other.GetComponent<PlayerDashCharges>();

        if (charges == null)
        {
            charges = other.GetComponentInParent<PlayerDashCharges>();
        }

        if (charges == null)
        {
            return;
        }

        charges.AddCharges(chargesToGive);
        gameObject.SetActive(false);
    }
}

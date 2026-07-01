using UnityEngine;

[DisallowMultipleComponent]
public class GroundAreaFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 position = transform.position;
        position.x = target.position.x;
        position.z = target.position.z;
        transform.position = position;
    }
}

using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject powerUpPrefab;
    [SerializeField] private float spawnInterval = 8f;
    [SerializeField] private Vector3 spawnAreaMin = new Vector3(-10f, 1f, -10f);
    [SerializeField] private Vector3 spawnAreaMax = new Vector3(10f, 1f, 10f);

    private static GameObject activePowerUp;
    private float timer;

    private void Start()
    {
        timer = spawnInterval;
    }

    private void Update()
    {
        if (activePowerUp == null || !activePowerUp.activeInHierarchy)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                SpawnPowerUp();
                timer = spawnInterval;
            }
        }
    }

    private void SpawnPowerUp()
    {
        Vector3 position = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
            Random.Range(spawnAreaMin.z, spawnAreaMax.z)
        );

        activePowerUp = Instantiate(powerUpPrefab, position, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Vector3 center = (spawnAreaMin + spawnAreaMax) / 2f;
        Vector3 size = spawnAreaMax - spawnAreaMin;
        Gizmos.DrawCube(center, size);
    }
}

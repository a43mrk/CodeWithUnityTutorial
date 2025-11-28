using System.Linq;
using UnityEngine;

public class SpawnBusFromSingleLane : MonoBehaviour
{
    [Header("Car Settings")]
    public GameObject carPrefab;
    public float forwardDistance = 20f; // distance ahead of trigger
    public bool facePlayer = true;  // should car face player?
    public bool singleUse = true;  // destroy trigger after spawn?

    private bool hasSpawned = false;

    void OnTriggerEnter(Collider other)
    {
        if(hasSpawned && singleUse) return;

        if(other.CompareTag("Player"))
        {
            SpawnCar(other.transform);
            hasSpawned = true;

            if (singleUse)
                Destroy(gameObject);
        }
    }

    void SpawnCar(Transform player)
    {
        // Base position ahead of trigger
        Vector3 spawnPos = transform.position + transform.forward * forwardDistance;

        // Rotation: either face player of align with road
        Quaternion spawnRot = facePlayer
            ?Quaternion.LookRotation(player.position - spawnPos) // face player
            :Quaternion.LookRotation(-transform.forward); // face opposite road direction

        Instantiate(carPrefab, spawnPos, spawnRot);
    }
}

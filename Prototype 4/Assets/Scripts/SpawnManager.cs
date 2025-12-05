using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    private float spawnRange = 9f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);

        Instantiate(
            enemyPrefab, new Vector3(spawnPosX, 0, spawnPosZ),
            enemyPrefab.transform.rotation
            );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Linq;
using UnityEditor;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject[] harderEnemies;
    public GameObject[] powerupPrefabs;
    public GameObject foodPrefab;
    public GameObject speedPrefab;
    private float spawnRange = 9f;

    /// <summary>
    /// Every time the player defeats a wave of enemies, more should rise to take their place.
    /// </summary>
    private int waveNumber = 1;

    public int ratio = 3;
    private int[] specialTotals;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeSpecialTotals();

        SpawnEnemyWave(waveNumber);
        SpawnPowerup();

    }

    private void InitializeSpecialTotals()
    {
        specialTotals = new int[harderEnemies.Length];
        for(int i = 0; i < harderEnemies.Length; i++)
        {
            specialTotals[i] = 0;
        }
    }

    private void SpawnSpeedPickup()
    {
        Instantiate(speedPrefab, GenerateSpawnPosition(), speedPrefab.transform.rotation);
    }

    private void SpawnFood()
    {
        Instantiate(foodPrefab, GenerateSpawnPosition(), foodPrefab.transform.rotation);
    }

    private void SpawnPowerup()
    {
        var prefab = powerupPrefabs[Random.Range(0, powerupPrefabs.Length)];
        Instantiate(prefab, GenerateSpawnPosition(), prefab.transform.rotation);
    }

    private void SpawnEnemyWave(int enemiesToSpawn)
    {

        for(int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(
                enemyPrefab,
                GenerateSpawnPosition(),
                enemyPrefab.transform.rotation
                );
        }

        if(enemiesToSpawn >= 2)
        {
            switch (Random.Range(0, 3))
            {
            case 0:
                SpawnFood();
                break;
            case 1:
                SpawnSpeedPickup();
                break;
            case 2:
                SpawnFood();
                SpawnSpeedPickup();
                break;
            default:
                return;

            }
        }

        CascadeSpecials();
    }

    /// <summary>
    /// Spawn Special Tier Enemies proportionally using ratio
    /// the ration regulates that to spawn a harder enemy for a bunch of normal enemies,
    /// that in turn a harder enemy is spawn if a bunch of hard enemy is spawn and so on...
    /// </summary>
    /// ratio holds the proportinal scale that will be used to calculate the harder enemies spawns
    /// specialTotals holds the sum of each hard enemy
    private void CascadeSpecials()
    {
        if (ratio <= 0) return;

        Debug.Log($"waveNumber: {waveNumber}, ratio: {ratio}, specialTotals: {specialTotals}");
        int neededTier0 = (waveNumber / ratio) + specialTotals.FirstOrDefault();
        Debug.Log($"neededTier0: {neededTier0}");
        SpawnSpecialsForTier(0, neededTier0);

        // tiers 1..N-1
        for (int tier = 1; tier < harderEnemies.Length; tier++)
        {
            int prerequisite = specialTotals[tier - 1];
            int needed = (prerequisite / ratio) + specialTotals[tier];
            SpawnSpecialsForTier(tier, needed);
        }

    }

    private void SpawnSpecialsForTier(int tier, int count)
    {
        if(count <= 0) return;
        var prefab = harderEnemies[tier];

        if(prefab == null)
        {
            Debug.LogWarning($"Special tier {tier} prefab is missing.");
            return;
        }

        Debug.Log($"count: {count}, tier: {tier}");;
        for(int i = 0; i < count; i++)
        {
            Instantiate(
                prefab,
                GenerateSpawnPosition(),
                prefab.transform.rotation
                );
            specialTotals[tier]++;
        }
        Debug.Log($"spawned Special Tier {tier}. Totals: {string.Join(",", specialTotals)}");
    }

    // Update is called once per frame
    void Update()
    {
        var enemyCount = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;
        if(enemyCount == 0)
        {
            waveNumber++;
            SpawnEnemyWave(waveNumber);
            SpawnPowerup();
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);

        return new Vector3(spawnPosX, 0, spawnPosZ);
    }
}

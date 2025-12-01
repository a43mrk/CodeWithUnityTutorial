using System;
using UnityEngine;

public enum SpawnSide
{
    Top, // y is 180degrees
    Left, // y is 90 degrees
    Right, // y is -90degrees
    Bottom // y is 0degrees
}

public class SpawnManager : MonoBehaviour
{
    public GameObject[] animalPrefabs;
    private float spawnRangeX = 16;
    private float spawnPosZ = 20;
    private float startDelay = 2;
    private float spawnInterval = 1.5f;
    // z range to spawn from left or right side: -1 ~ 16
    private float lowerBound = -1f;
    private float upperBound = 16f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SpawnRandomAnimal", startDelay, spawnInterval);
    }

    // Update is called once per frame
    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.S))
        // {
        //     SpawnRandomAnimal();
        // }
    }

    private void SpawnRandomAnimal()
    {
        foreach(SpawnSide side in Enum.GetValues(typeof(SpawnSide)))
        {
            // Randomly generate animal index and spawn position
            int animalIndex = UnityEngine.Random.Range(0, animalPrefabs.Length);
            var prefab = animalPrefabs[animalIndex];

            var (rotation, spawnPos) = GetRotationAndSpawnPosition(side, prefab.transform.rotation);

            Instantiate(prefab,
                spawnPos,
                rotation
            );
        }
    }

    private (Quaternion rotation, Vector3 spawnPos) GetRotationAndSpawnPosition(SpawnSide side, Quaternion rotation)
    {
        switch(side)
        {
            case SpawnSide.Top:
                rotation = Quaternion.Euler(0, 180, 0);
                return (
                    rotation,
                    new(UnityEngine.Random.Range(-spawnRangeX, spawnRangeX), 0, spawnPosZ)
                );
            case SpawnSide.Bottom:
                rotation = Quaternion.Euler(0, 0, 0);
                return (
                    rotation,
                    new(UnityEngine.Random.Range(-spawnRangeX, spawnRangeX), 0, lowerBound)
                    );
            case SpawnSide.Left:
                rotation = Quaternion.Euler(0, -90, 0);
                return (
                    rotation,
                    new(spawnRangeX, 0, UnityEngine.Random.Range(lowerBound, upperBound))
                    );
            case SpawnSide.Right:
                rotation = Quaternion.Euler(0, 90, 0);
                return (
                    rotation,
                    new(-spawnRangeX, 0, UnityEngine.Random.Range(lowerBound, upperBound))
                    );
            default:
                throw new Exception("impossible condition");

        }
    }
}

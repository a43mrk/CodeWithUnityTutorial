using TMPro;
using UnityEngine;

public class DetectCollisions : MonoBehaviour
{
    public float life = 3.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        DestroyOutOfBounds.OnAnimalDestroyed += ReduceLife;
    }

    void OnDisable()
    {
        DestroyOutOfBounds.OnAnimalDestroyed -= ReduceLife;
    }

    void OnTriggerEnter(Collider other)
    {
        ReduceLife();
        Debug.Log($"collided with: {other.gameObject.name}");

        if (life <= 0 && other.gameObject.CompareTag("Projectile"))
        {
            SpawnManager.Instance.AddScore(10);
        }
    }

    private void CheckForLife()
    {
        if (life <= 0)
        {

            if (gameObject.CompareTag("Player"))
                Debug.Log("Game Over!");

            Debug.Log($"Destroying {gameObject.name}, has tag: {gameObject.tag}");
            Destroy(gameObject);
            // Destroy(other.gameObject);

        }
    }

    public void ReduceLife()
    {
        --life;
        Debug.Log($"reduces {gameObject.name} life: {life}");

        CheckForLife();
    }
}

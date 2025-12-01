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

    void OnTriggerEnter(Collider other)
    {
        ReduceLife(1);
        Debug.Log($"collided with: {other.gameObject.name}");

        if(life <= 0)
        {
            if(other.gameObject.CompareTag("Projectile"))
            {
                SpawnManager.Instance.AddScore(10);
            }

            if(gameObject.CompareTag("Player"))
                Debug.Log("Game Over!");

            Destroy(gameObject);
            // Destroy(other.gameObject);

        }
    }

    public void ReduceLife(int amount)
    {
        life -= amount;
        Debug.Log($"reduces {gameObject.name} life: {life}");
    }
}

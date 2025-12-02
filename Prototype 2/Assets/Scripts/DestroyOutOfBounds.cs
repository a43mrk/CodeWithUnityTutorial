using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    private float topBound = 35;
    private float bottomBound = -15;

    private float sideRange = 25f;
    // public static event System.Action OnAnimalDestroyed;
    private DetectCollisions player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player")?.GetComponent<DetectCollisions>();
    }

    // Update is called once per frame
    void Update()
    {
        var z = transform.position.z;
        var x = transform.position.x;
        if(z > topBound || z < bottomBound || x > sideRange || x < -sideRange)
        {
            Destroy(gameObject);
            if(gameObject.CompareTag("Animal") )
            {
                // reduce players life by 1
                // OnAnimalDestroyed?.Invoke();
                Debug.Log("reducing life for animal escape");
                player?.ReduceLife();
            }
            if(gameObject.CompareTag("Player"))
                Debug.Log("Game Over!");
        }
    }
}

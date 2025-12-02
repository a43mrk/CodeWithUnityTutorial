using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetectCollisions : MonoBehaviour
{
    private float lifeRemain = 0f;
    public GameObject hugerBarPrefab;
    public float totalLife = 3.0f;
    private Slider hungerBar;

    void Awake()
    {
        hungerBar = Instantiate(hugerBarPrefab, gameObject.transform).GetComponentInChildren<Slider>();
        lifeRemain += totalLife;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        hungerBar.value = (totalLife - lifeRemain)/totalLife;
    }

    // void OnEnable()
    // {
    //     DestroyOutOfBounds.OnAnimalDestroyed += ReduceLife;
    // }

    // void OnDisable()
    // {
    //     DestroyOutOfBounds.OnAnimalDestroyed -= ReduceLife;
    // }

    void OnTriggerEnter(Collider other)
    {
        if(gameObject.CompareTag("Player") && other.gameObject.CompareTag("Projectile"))
        {
            Debug.Log("**ignoring projectile collision with player**");
            return;
        }

        Debug.Log($"collided with: {other.gameObject.name}");
        ReduceLife();

        if (lifeRemain <= 0 && other.gameObject.CompareTag("Projectile"))
        {
            SpawnManager.Instance.AddScore(10);
        }
    }

    private void CheckForLife()
    {
        if (lifeRemain <= 0)
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
        --lifeRemain;
        Debug.Log($"reduces {gameObject.name} life: {lifeRemain}");

        CheckForLife();
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetectCollisions : MonoBehaviour
{
    private float lifeRemain = 0f;
    public GameObject progressBarPrefab;
    public float totalLife = 3.0f;
    private float currentFeedAmount = 0f;
    public float amountToBeFeed = 3.0f;
    private Slider _progressBar;

    void Awake()
    {
        _progressBar = Instantiate(progressBarPrefab, gameObject.transform).GetComponentInChildren<Slider>();
        lifeRemain += totalLife;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(gameObject.CompareTag("Player"))
            _progressBar.value = (totalLife - lifeRemain)/totalLife;

        if(gameObject.CompareTag("Animal"))
            _progressBar.value =   currentFeedAmount / amountToBeFeed;
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
        if(gameObject.CompareTag("Player"))
        {
            if(other.gameObject.CompareTag("Projectile"))
            {
                Debug.Log("**ignoring projectile collision with player**");
                return;
            }

            Debug.Log($"collided with: {other.gameObject.name}");
            ReduceLife(other);
        } else if (gameObject.CompareTag("Animal") && other.gameObject.CompareTag("Projectile"))
        {
            Feed();
        }
    }

    private void CheckForLife(Collider other)
    {
        if (lifeRemain <= 0)
        {

            if (gameObject.CompareTag("Player"))
            {
                Debug.Log($"Destroying {gameObject.name}, has tag: {gameObject.tag}");

                Debug.Log("Game Over!");
                Destroy(gameObject);
            }


            // Instead of destroying the projectile when it collides with an animal
            //Destroy(other.gameObject); 

            // Just deactivate the food and destroy the animal
            other?.gameObject.SetActive(false);
            Debug.Log($"Deativate {gameObject.name}, has tag: {gameObject.tag}");

        }
    }

    public void ReduceLife(Collider other)
    {
        --lifeRemain;
        Debug.Log($"reduces {gameObject.name} life: {lifeRemain}");

        CheckForLife(other);
    }

    public void Feed()
    {
        Debug.Log("feeding");
        ++currentFeedAmount;

        if(currentFeedAmount >= amountToBeFeed)
        {
            Debug.Log($"fully feed {currentFeedAmount}");
            SpawnManager.Instance.AddScore(10);
            Destroy(gameObject);
        }
    }
}

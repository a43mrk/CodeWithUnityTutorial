using UnityEngine;

public class MissedShootDetector : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject missingBallsTeleportDirection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Ball"))
            return;

        gameManager.IncrementMissedShoots();

        other.transform.position = missingBallsTeleportDirection.transform.position;
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if(rb == null)
        {
            Debug.LogError("Ball prefab must have a Rigidbody.");
        }

        Debug.Log("missed ball speed: " + rb.linearVelocity);

        Vector3 direction = missingBallsTeleportDirection.transform.up;
        rb.AddForce(direction * 50, ForceMode.Impulse);
    }
}

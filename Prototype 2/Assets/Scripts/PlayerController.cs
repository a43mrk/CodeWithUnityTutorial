using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalInput;
    public float verticalInput;
    public float speed = 10.0f;
    public float xRange = 16f;

    // Z limit: 16.5 ~ -2
    public float lowerBound = -1f;
    public float upperBound = 16f;

    public GameObject projectilePrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // Launch a projectile from the player
            Instantiate(
                projectilePrefab,
                transform.position + new Vector3(0f, 1.0f, 1.5f),
                Quaternion.identity);
        }

        ProcessMove();
    }

    private void ProcessMove()
    {
        // Keep the player in bounds
        if (transform.position.x < -xRange)
        {
            transform.position = new Vector3(-xRange, transform.position.y, transform.position.z);
        }

        if (transform.position.x > xRange)
        {
            transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
        }

        if (transform.position.z >= upperBound)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, upperBound);
        }

        if(transform.position.z <= lowerBound)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, lowerBound);
        }

        horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * speed);

        verticalInput = Input.GetAxis("Vertical");
        transform.Translate(Vector3.forward * verticalInput * Time.deltaTime * speed);

    }
}

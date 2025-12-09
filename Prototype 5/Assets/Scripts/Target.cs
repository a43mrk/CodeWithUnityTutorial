using UnityEngine;

public class Target : MonoBehaviour
{
    private Rigidbody targetRb;
    private float minSpeed = 12;
    private float maxSpeed = 16;
    private float maxTorque = 10;
    private float xRange = 4;
    private float ySpawnPos = -6;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetRb = GetComponent<Rigidbody>();

        // throw the object upwards
        targetRb.AddForce(RandomForce(), ForceMode.Impulse);
        // make it rotate or spin
        targetRb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);

        // random place for the object to spawn
        transform.position = RandomSpawnPos();
    }

    private Vector3 RandomSpawnPos()
    {

        return new Vector3(Random.Range(-xRange, xRange), -ySpawnPos); // no need for z axis here, since we use 2d camera for this game.
    }

    private float RandomTorque()
    {
        return Random.Range(-maxTorque, maxTorque);
    }

    private Vector3 RandomForce()
    {
        return Vector3.up * Random.Range(minSpeed, maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// will be triggered when the objects collides with the sensor set at bottom of the scene.
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}

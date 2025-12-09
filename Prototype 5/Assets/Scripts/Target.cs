using UnityEngine;

public class Target : MonoBehaviour
{
    private Rigidbody targetRb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetRb = GetComponent<Rigidbody>();

        // throw the object upwards
        targetRb.AddForce(Vector3.up * Random.Range(12, 16), ForceMode.Impulse);
        // make it rotate or spin
        targetRb.AddTorque(Random.Range(-10,10), Random.Range(-10, 10), Random.Range(-10, 10), ForceMode.Impulse);

        // random place for the object to spawn
        transform.position = new Vector3(Random.Range(-4, 4), -6); // no need for z axis here, since we use 2d camera for this game.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

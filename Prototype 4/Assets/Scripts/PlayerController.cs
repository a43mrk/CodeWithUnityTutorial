using System.Collections;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject focalPoint;
    private bool hasPowerup = false;
    private float powerupStrength = 15.0f;
    public float speed = 5.0f;
    public GameObject powerupIndicator;
    public Transform pfRocket;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    // Update is called once per frame
    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * speed * forwardInput);

        if(hasPowerup)
        {
            powerupIndicator.transform.position = transform.position + new Vector3(0, -.5f, 0);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            // use only the camera's yaw
            float yaw = Camera.main.transform.eulerAngles.y;

            // Build a flat forward direction
            Vector3 flatForward = Quaternion.Euler(0, yaw, 0) * Vector3.forward;
            flatForward.Normalize();

            // TODO: fix the instantiation of the distance of rocket from the player
            var rot = Quaternion.LookRotation(flatForward, Vector3.up);

            var position = transform.position + flatForward * 1.5f;
            Instantiate(pfRocket, position, rot);
        }

        Debug.DrawRay(transform.position, Camera.main.transform.forward * 5f, Color.red);
        Debug.DrawRay(transform.position, Camera.main.transform.right * 5f, Color.green);
        Debug.DrawRay(transform.position, Camera.main.transform.up * 5f, Color.blue);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            Destroy(other.gameObject);

            powerupIndicator.gameObject.SetActive(true);
        }

        // Eating food adds one to mass
        if(other.CompareTag("Food"))
        {
            Destroy(other.gameObject);
            playerRb.mass += 1;
        }

        if(other.CompareTag("Speed"))
        {
            Destroy(other.gameObject);
            speed += 5.0f;
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        /// The powerup will only come into play in a very particular circumstance:
        /// when the player has a powerup AND they collide with an enemy
        /// - so we’ll first test for that very specific condition.
        if(collision.gameObject.CompareTag("Enemy") && hasPowerup)
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position);

            Debug.Log("Collided with" + collision.gameObject.name + " with powerup set to " + hasPowerup);
            enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);

            // limit the powerup by time
            StartCoroutine(PowerupCountdownRoutine());
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(5);
        hasPowerup = false;
        Debug.Log("Powerup effect vanishes...");
        powerupIndicator.gameObject.SetActive(false);
    }
}

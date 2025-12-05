using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject focalPoint;
    private bool hasPowerup = false;
    private float powerupStrength = 15.0f;
    public float speed = 5.0f;
    public GameObject powerupIndicator;

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
            powerupIndicator.transform.position = transform.position + new Vector3(0, -.5f, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            Destroy(other.gameObject);

            powerupIndicator.gameObject.SetActive(true);
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

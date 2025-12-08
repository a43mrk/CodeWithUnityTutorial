using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject focalPoint;
    private bool hasPowerup = false;
    private float powerupStrength = 15.0f;
    public float speed = 5.0f;
    public float jumpForce = 8f;
    public float slamForce = -30f;
    public float shockwaveRadius = 6f;
    public float shockwaveForce = 25f;
    public GameObject powerupIndicator;
    public Transform pfRocket;
    private bool canLaunchRockets = false;
    private bool hasImpactPowerup = false;
    private bool isGrounded = true;
    private bool isSlamming = false;
    public GameObject rocketPowerupIndicator;
    public GameObject impactPowerupIndicator;
    private float lastCtrPressTime = -1f;
    private float comboMaxDelay = 1f;

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

        if(canLaunchRockets)
        {
            rocketPowerupIndicator.transform.position = transform.position + new Vector3(0, -.5f, 0);
        }

        if(hasImpactPowerup)
        {
            impactPowerupIndicator.transform.position = transform.position + new Vector3(0, -.5f, 0);

            if(isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                    playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    isGrounded = false;
            }
            else if(!isGrounded && Input.GetKeyDown(KeyCode.Space) && !isSlamming)
            {
                isSlamming = true;
                playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z); // cancel upward motion
                playerRb.AddForce(Vector3.down * MathF.Abs(slamForce), ForceMode.Impulse);
            }
        }

        if(canLaunchRockets && Input.GetKeyDown(KeyCode.Space))
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

        if(Input.GetKeyDown(KeyCode.LeftControl))
            lastCtrPressTime = Time.time;

        if((Time.time - lastCtrPressTime <= comboMaxDelay) && Input.GetKeyDown(KeyCode.I))
        {
            ActivateImpactPowerup();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        Debug.DrawRay(transform.position, Camera.main.transform.forward * 5f, Color.red);
        Debug.DrawRay(transform.position, Camera.main.transform.right * 5f, Color.green);
        Debug.DrawRay(transform.position, Camera.main.transform.up * 5f, Color.blue);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Powerup"))
        {
            if(other.gameObject.name.Contains("Rocket"))
            {
                Destroy(other.gameObject);
                ActivateRocketPowerup();
            }
            else if(other.gameObject.name.Contains("Impact"))
            {
                Destroy(other.gameObject);
                ActivateImpactPowerup();
            }
            else
            {
                Destroy(other.gameObject);
                ActivatePowerup();
            }
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

    private void ActivateRocketPowerup()
    {
        canLaunchRockets = true;
        rocketPowerupIndicator.gameObject.SetActive(true);

        StartCoroutine(RocketPowerupCooldownRoutine());
    }

    private void ActivateImpactPowerup()
    {
        hasImpactPowerup = true;
        impactPowerupIndicator.SetActive(true);
        StartCoroutine(ImpactPowerupCooldownRoutine());
    }

    private void ActivatePowerup()
    {
        hasPowerup = true;
        powerupIndicator.gameObject.SetActive(true);

        // limit the powerup by time
        StartCoroutine(PowerupCountdownRoutine());
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

        }

        if(collision.gameObject.CompareTag("Ground"))
        {
            if(isSlamming)
            {
                createShockWave();
            }

            isGrounded = true;
            isSlamming = false;
        }
    }

    private void createShockWave()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, shockwaveRadius);
        Debug.Log($"Shockwave detected {hits.Length} colliders.");

        foreach(Collider hit in hits)
        {
            Debug.Log($"hit tag is: {hit.gameObject.tag }");
            // if(hit.gameObject.CompareTag("Enemy"))
            // {
                Rigidbody enemyRb = hit.GetComponentInParent<Rigidbody>();
                if(enemyRb)
                {
                    Vector3 dir = (hit.transform.position - transform.position).normalized;
                    Debug.Log($"Applying force to enemy {hit.name}, direction: {dir}");
                    enemyRb.AddForce(dir * shockwaveForce, ForceMode.Impulse);
                }
            // }
        }


        // TODO: Add VFX / Camera shake / sound here
        Debug.Log("Shockwave triggered!");
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(5);
        hasPowerup = false;
        Debug.Log("Powerup effect vanishes...");
        powerupIndicator.gameObject.SetActive(false);
    }

    IEnumerator RocketPowerupCooldownRoutine()
    {
        yield return new WaitForSeconds(5);
        canLaunchRockets = false;
        rocketPowerupIndicator.gameObject.SetActive(false);
    }
    private IEnumerator ImpactPowerupCooldownRoutine()
    {
        yield return new WaitForSeconds(5);
        hasImpactPowerup = false;
        impactPowerupIndicator.gameObject.SetActive(false);
    }
}

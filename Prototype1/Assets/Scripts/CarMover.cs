using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 10f;
    public float lifetime = 10f;
    public float collisionDelay = 2f;
    private float timer = 0f;
    private bool isColliding = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Move the car forward along its local forwar axis
        transform.Translate(Vector3.forward * speed *Time.deltaTime);

        // Lifetime control
        timer += Time.deltaTime;
        if(timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the car collides with the player, destroy it
        if(collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DestroyAfterDelay());
        }

    }

    private IEnumerator DestroyAfterDelay()
    {
        isColliding = true; // stop movement
        yield return new WaitForSeconds(collisionDelay);
        Destroy(gameObject);
    }
}

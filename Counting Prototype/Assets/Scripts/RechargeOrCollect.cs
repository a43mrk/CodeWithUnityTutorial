using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RechargeOrCollect : MonoBehaviour
{
    public GameObject collectorsSpawnDirection;

    private readonly HashSet<GameObject> objectsInContact = new HashSet<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        // c to collect
        if(Input.GetKeyDown(KeyCode.C))
        {
            if(objectsInContact.Any())
                CollectPayout(objectsInContact.FirstOrDefault());
        }
        // r to use the available balls into credits
        else if(Input.GetKeyDown(KeyCode.R))
        {
            // TODO: Add to credits
            // Destroy(other.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Ball"))
            return;

        objectsInContact.Add(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        objectsInContact.Remove(other.gameObject);
    }

    private void CollectPayout(GameObject ball)
    {
        ball.transform.position = collectorsSpawnDirection.transform.position;

        Debug.Log("instantiating payout ball: " + ball.gameObject.GetInstanceID());

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if(rb == null)
        {
            Debug.LogError("Ball prefab must have a Rigidbody.");
            return;
        }

        Vector3 direction = collectorsSpawnDirection.transform.up;
        rb.AddForce(direction * 100, ForceMode.Impulse);
    }

}

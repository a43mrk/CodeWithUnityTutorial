using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public void OnCollectAction(InputAction.CallbackContext context)
    {
        if(objectsInContact.Any())
            CollectPayout(objectsInContact.FirstOrDefault());
    }

    public GameObject TakeABall()
    {
        var item = objectsInContact.FirstOrDefault();
        if(item != null)
        {
            objectsInContact.Remove(item);
            return item;
        }
        else
            return null;
    }

    private void CollectPayout(GameObject ball)
    {
        ball.transform.position = collectorsSpawnDirection.transform.position;

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

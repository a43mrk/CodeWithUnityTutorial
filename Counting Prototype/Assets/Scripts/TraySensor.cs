using System.Collections.Generic;
using UnityEngine;

public class TraySensor : MonoBehaviour
{
    public readonly HashSet<GameObject> ballsInTray = new();

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
        if(other.CompareTag("Ball"))
        {
            ballsInTray.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Ball"))
        {
            ballsInTray.Remove(other.gameObject);
        }
    }

}

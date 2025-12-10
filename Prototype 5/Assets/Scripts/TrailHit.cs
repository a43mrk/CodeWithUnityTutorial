using UnityEngine;

public class TrailHit : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Target t = other.gameObject.GetComponent<Target>();
        if(t)
        {
            t.TakeDown();
        }
    }
}

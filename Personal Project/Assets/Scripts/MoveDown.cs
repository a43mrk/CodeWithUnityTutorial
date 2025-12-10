using UnityEngine;

public class MoveDown : MonoBehaviour
{
    public float speed = 5.0f;

    private float zDestroy = -10.0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        // rb.AddForce(Vector3.forward * -speed);
        transform.Translate(Vector3.forward * -speed * Time.deltaTime);


        // this make sure objects are destroyed when they leave the screen
        if(transform.position.z <= zDestroy)
        {
            Destroy(gameObject);
        }
    }
}

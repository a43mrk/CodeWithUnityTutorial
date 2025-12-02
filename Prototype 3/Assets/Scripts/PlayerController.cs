using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 100f;
    public float gravityModifier;
    private Rigidbody playerRb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        Physics.gravity *= gravityModifier;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}

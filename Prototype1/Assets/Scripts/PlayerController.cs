using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 20.0f;
    [SerializeField] private float turnSpeed = 45.0f;

    [Header("Custom Input Keys")]
    [SerializeField] private KeyCode forwardKey = KeyCode.W;
    [SerializeField] private KeyCode backwardKey = KeyCode.S;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;

    private float horizontalInput;
    private float forwardInput;

    // FixedUpdate is better for physics calculations
    void FixedUpdate()
    {
        // Get custom key inputs
        horizontalInput = 0f;
        forwardInput = 0f;

        // Forward/Backward input
        if(Input.GetKey(forwardKey))
            forwardInput += 1f;
        if(Input.GetKey(backwardKey))
            forwardInput -= 1f;

        // Left/Right input
        if(Input.GetKey(leftKey))
            horizontalInput -= 1f;
        if(Input.GetKey(rightKey))
            horizontalInput += 1f;

        // Move the vehicle forward based on vertical input
        transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);

        // Rotates the car based on horizontal input
        transform.Rotate(Vector3.up, turnSpeed * horizontalInput * Time.deltaTime);
    }
}

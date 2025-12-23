using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SaloonDoorWing : MonoBehaviour
{
    public float returnStrength = 50f;
    public float damping = 8f;

    // Local hinge axis (example: Z)
    public Vector3 localAxis = Vector3.forward;

    private Rigidbody rb;
    private Quaternion restRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        restRotation = transform.localRotation;

        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        // Rotation difference
        Quaternion delta = restRotation * Quaternion.Inverse(transform.localRotation);
        delta.ToAngleAxis(out float angle, out _);

        // Normalize to [-180, +180]
        if (angle > 180f)
            angle -= 360f;

        // Axis in world space
        Vector3 axisWorld = transform.parent
            ? transform.parent.TransformDirection(localAxis.normalized)
            : transform.TransformDirection(localAxis.normalized);

        // Angular velocity projected on hinge axis
        float velOnAxis = Vector3.Dot(rb.angularVelocity, axisWorld);

        // Spring-damper torque
        float torque =
            angle * Mathf.Deg2Rad * returnStrength
            - velOnAxis * damping;

        rb.AddTorque(axisWorld * torque, ForceMode.Acceleration);
    }
}

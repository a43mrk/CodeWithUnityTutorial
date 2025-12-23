using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WingAutoReturn : MonoBehaviour
{
    [Header("Return Behavior")]
    public float returnStrength = 60f;
    public float damping = 10f;

    [Header("Rotation Axis (Local)")]
    public Vector3 localRotationAxis = Vector3.forward;

    private Rigidbody rb;
    private Quaternion startLocalRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startLocalRotation = transform.localRotation;

        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void FixedUpdate()
    {
        // Current vs target.rotation
        Quaternion current = transform.localRotation;
        Quaternion delta = startLocalRotation * Quaternion.Inverse(current);

        delta.ToAngleAxis(out float angle, out Vector3 _);

        if (angle > 180f)
            angle -= 360f;

        Vector3 axisWorld = transform.parent
            ?transform.parent.TransformDirection(localRotationAxis.normalized)
            :transform.TransformDirection(localRotationAxis.normalized);
        
        float angularVelocityOnAxis = Vector3.Dot(rb.angularVelocity, axisWorld);

        float correctiveTorque = (angle * Mathf.Deg2Rad * returnStrength)
            - (angularVelocityOnAxis * damping);

        rb.AddTorque(axisWorld * correctiveTorque, ForceMode.Acceleration);
    }
}

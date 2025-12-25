using UnityEngine;

public class PachinkoBall : MonoBehaviour
{
    [Header("Impulse Tuning")]
    public float minImpulse = 0.04f;
    public float maxImpulse = 2.0f;
    [Header("Audio")]
    public float minVolume = 0.05f;
    public float maxVolume = 0.8f;
    public float pitchVariation = 0.08f;
    [Header("Spam Control")]
    public float minInterval = 0.02f;

    private AudioSource ballCollideWithPinFx;

    private int lastCollidedNailId = 0;
    float lastPlayTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        ballCollideWithPinFx = GetComponent<AudioSource>();
        ballCollideWithPinFx.spatialBlend = 1f; // 3d
        ballCollideWithPinFx.playOnAwake = false;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.CompareTag("Nail")) return;

        Debug.Log("Collided with: " + collision.gameObject.name);
        if(lastCollidedNailId != collision.gameObject.GetInstanceID())
        {
            float impulse = collision.impulse.magnitude;
            Debug.Log("impulse is: " + impulse);

            if(impulse < minImpulse)
                return;

            if(Time.time - lastPlayTime < minInterval)
                return;
            
            lastPlayTime = Time.time;

            float t = Mathf.InverseLerp(minImpulse, maxImpulse, impulse);
            float volume = Mathf.Lerp(minVolume, maxVolume, t);

            ballCollideWithPinFx.volume = volume;
            ballCollideWithPinFx.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);

            lastCollidedNailId = collision.gameObject.GetInstanceID();
            if(ballCollideWithPinFx.isPlaying)
                ballCollideWithPinFx.Stop();

            ballCollideWithPinFx.PlayOneShot(ballCollideWithPinFx.clip);
        }
    }

    public static float KineticEnergy(Rigidbody rb){
        // mass in kg, velocity in meters per second, result is joules
        return 0.5f*rb.mass*Mathf.Pow(rb.linearVelocity.magnitude,2); 
    }
}

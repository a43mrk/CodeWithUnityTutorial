using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class CircularPachinkoPowerController : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("UI")]
    [SerializeField] private Slider radialSlider;

    [Header("Value Range")]
    [SerializeField] private int minValue = 5;
    [SerializeField] private int maxValue = 100;

    [Header("Decay (Optional Difficulty)")]
    [SerializeField] private bool enableDecay = true;
    [SerializeField] private float decaySpeedWhileHolding = 0.05f; // slower decay when holding
    [SerializeField] private float decaySpeedWhileIdle = 0.2f;     // faster decay when idle


    [Header("Instability / Noise")]
    [SerializeField] private bool enableInstability = true;
    [SerializeField] private float noiseStrength = 0.06f; // fraction of max (6%)
    [SerializeField] private float noiseSpeed = 2f;
    [SerializeField] private int noiseSeed = 12345;

    [Header("Force Output")]
    [SerializeField] private float baseShootForce = 1000f;

    [Header("Input Scaling")]
    [SerializeField] private float dragSensitivity = 0.2f; // tweak this for integer scale

    [Header("Auto Fire")]
    [SerializeField] private float fireIntervalSeconds = 1f;

    [SerializeField]
    public UnityEvent<float> OnFireRequested;

    private bool isInteracting;
    private int rawValue;
    private float noiseOffset;
    private System.Random seededRandom;
    private float fireTimer;

    void Awake()
    {
        radialSlider.minValue = 0;
        radialSlider.maxValue = 100;
        radialSlider.wholeNumbers = true;

        seededRandom = new System.Random(noiseSeed);
        noiseOffset = (float)seededRandom.NextDouble() * 100f;

        rawValue = Mathf.Clamp(Mathf.RoundToInt(radialSlider.value), minValue, maxValue);
    }

    void Update()
    {
        if (rawValue <= 0)
        {
            rawValue = 0;
            radialSlider.value = 0;
            fireTimer = 0f;
            return;
        }

        // Decay logic (holding vs idle)
        if (enableDecay)
        {
            float decaySpeedToUse = isInteracting
                ? decaySpeedWhileHolding
                : decaySpeedWhileIdle;

            float decayAmount = decaySpeedToUse * Time.deltaTime * 100f;
            rawValue -= Mathf.FloorToInt(decayAmount);
            if (rawValue < 0) rawValue = 0;
        }

        float displayedValue = rawValue;

        if (enableInstability)
        {
            float noise = Mathf.PerlinNoise(
                noiseOffset,
                Time.time * noiseSpeed
            );

            noise = (noise - 0.5f) * 2f;
            noise *= noiseStrength * maxValue;

            displayedValue = Mathf.Clamp(rawValue + noise, 0, maxValue);
        }

        radialSlider.value = Mathf.RoundToInt(displayedValue);

        HandleAutoFire();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        isInteracting = true;
        rawValue = Mathf.Clamp(Mathf.RoundToInt(radialSlider.value), minValue, maxValue);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isInteracting)
            return;

        int delta = Mathf.RoundToInt(eventData.delta.y * dragSensitivity);
        rawValue += delta;
        rawValue = Mathf.Clamp(rawValue, minValue, maxValue);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isInteracting = false;
        // Manual firing removed - only auto-fire mechanism is used
    }

    // FIXED: Now properly invokes OnFireRequested event
    private void FireBall(float force)
    {
        Debug.Log($"Pachinko Shot Force: {force}");
        OnFireRequested?.Invoke(force);
    }

    // FIXED: Auto-fire continues even while holding (user adjusts, balls keep firing)
    private void HandleAutoFire()
    {
        // FIXED: Use minValue instead of hardcoded 1
        if (rawValue < minValue)
        {
            fireTimer = 0f;
            return;
        }

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireIntervalSeconds)
        {
            fireTimer -= fireIntervalSeconds;

            float normalizedValue = rawValue / (float)maxValue;
            float force = baseShootForce * normalizedValue;
            
            // FIXED: Now uses unified FireBall method for consistency
            FireBall(force);
        }
    }


    /// <summary>
    /// Updates raw slider value based on analog stick input vector (X,Y)
    /// Maps stick magnitude (0 to 1) to slider integer range (minValue to maxValue)
    /// </summary>
    /// <param name="stickInput">Analog stick Vector2 input, typically from -1 to 1</param>
    public void UpdateValueFromAnalogStick(Vector2 stickInput)
    {
        float magnitude = Mathf.Clamp01(stickInput.magnitude);
        rawValue = Mathf.RoundToInt(Mathf.Lerp(minValue, maxValue, magnitude));
    }

    /// <summary>
    /// Updates raw slider value based on pointer drag delta input (e.g. mouse/touch delta)
    /// Delta is expected in pixels or normalized units; scaled by dragSensitivity
    /// </summary>
    /// <param name="dragDelta">Drag delta vector (usually eventData.delta)</param>
    public void UpdateValueFromPointerDrag(Vector2 dragDelta)
    {
        int deltaInt = Mathf.RoundToInt(dragDelta.y * dragSensitivity);
        rawValue += deltaInt;
        rawValue = Mathf.Clamp(rawValue, minValue, maxValue);
    }

    // Called by InputAction for stick move (binding: <Gamepad>/leftStick)
    public void OnAnalogStickInput(InputAction.CallbackContext context)
    {
        Vector2 stickInput = context.ReadValue<Vector2>();
        UpdateValueFromAnalogStick(stickInput);
    }

    // Called by InputAction for pointer drag (binding: <Pointer>/delta)
    public void OnPointerDrag(InputAction.CallbackContext context)
    {
        Vector2 dragDelta = context.ReadValue<Vector2>();
        UpdateValueFromPointerDrag(dragDelta);
    }

}
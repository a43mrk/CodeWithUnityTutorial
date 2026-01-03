using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DragCameraController : MonoBehaviour
{
    public GameObject pachinkoMachine;

    [Header("References")]
    public Camera cam;
    public Transform cameraPivot;

    [Header("Movement")]
    public float moveSpeed = 10f;
    public float panSpeed = 0.01f;
    public float elevateSpeed = 5f;

    [Header("Rotation")]
    public float rotationSpeed = 0.2f;
    [Header("Pitch Limits")]
    public float minPitch = -80f;
    public float maxPitch = 80f;

    [Header("Zoom")]
    public float zoomSpeed = 5f;
    public float minZoom = 3f;
    public float maxZoom = 25f;

    public float heightSpeed = 5f;

    [Header("Invert Axis")]
    public bool invertY = false;

    [Header("Rotation Sensitivity")]
    public bool useSensitivityScaling = true;
    public float mouseSensitivity = 1.0f;
    public float touchSensitivity = 0.4f;

    [Header("Rotation Smoothing")]
    public bool useRotationSmoothing = true;
    public float rotationSmoothTime = 0.08f;


    InputAction move;
    InputAction pointerDelta;
    InputAction pointerPress;
    InputAction zoom;
    InputAction rotate;
    InputAction rotatePress;
    InputAction elevate;

    bool blockByUI;
    float pitch;
    float yawVelocity;
    float pitchVelocity;


    void Awake()
    {
        var machine = pachinkoMachine.GetComponent<PachinkoMachineManager>();
        var playerInput = machine.GetPlayerInput();
        pointerDelta = playerInput.InGame.PointerDelta;
        pointerPress = playerInput.InGame.PointerPress;
        zoom = playerInput.InGame.Zoom;
        move = playerInput.InGame.Move;
        rotate = playerInput.InGame.Rotate;
        rotatePress = playerInput.InGame.RotatePress;
        elevate = playerInput.InGame.Elevate;

        pitch = cameraPivot.localEulerAngles.x;

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        HandleUIBLock();
        HandleKeyboardMove();
        HandlePan();
        HandleRotation();
        HandleZoom();
        HandleElevation();
    }

    void LateUpdate()
    {
        if(Touchscreen.current == null)
            return;
        
        if(Touchscreen.current.touches.Count < 2)
            return;
        
        var t0 = Touchscreen.current.touches[0];
        var t1 = Touchscreen.current.touches[1];

        if(!t0.press.isPressed || !t1.press.isPressed)
            return;
        
        Vector2 prevPos0 = t0.position.ReadValue() - t0.delta.ReadValue();
        Vector2 prevPos1 = t1.position.ReadValue() - t1.delta.ReadValue();

        float prevDist = Vector2.Distance(prevPos0, prevPos1);
        float currentDist = Vector2.Distance(t0.position.ReadValue(), t1.position.ReadValue());

        float diff = currentDist - prevDist;

        cam.orthographicSize = Mathf.Clamp(
            cam.orthographicSize - diff * 0.01f,
            minZoom,
            maxZoom
        );
    }

    void HandleUIBLock()
    {
        if (pointerPress.WasPressedThisFrame())
        {
            blockByUI = EventSystem.current != null
            &&
            EventSystem.current.IsPointerOverGameObject();
        }

        if(pointerPress.WasReleasedThisFrame())
        {
            blockByUI = false;
        }
    }

    void HandleKeyboardMove()
    {
        Vector2 input = move.ReadValue<Vector2>();
        if(input.sqrMagnitude < 0.01f)
            return;
        
        Vector3 dir = transform.forward * input.y +
            transform.right * input.x;
        
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float scroll = zoom.ReadValue<float>();
        
        if(Mathf.Abs(scroll) < 0.01f)
            return;
        
        if(cam.orthographic)
        {
            cam.orthographicSize = Mathf.Clamp(
                cam.orthographicSize - scroll * zoomSpeed * Time.deltaTime,
                minZoom,
                maxZoom
            );
        }
        else
        {
            cam.fieldOfView = Mathf.Clamp(
                cam.fieldOfView - scroll * zoomSpeed * Time.deltaTime,
                minZoom,
                maxZoom
            );
        }
    }

    private void HandlePan()
    {
        if(blockByUI || !pointerPress.IsPressed())
            return;

        // if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        //     return;

        // if(!pointerPress.IsPressed())
        //     return;

        // if (
        //     Touchscreen.current != null
        //     && Touchscreen.current.primaryTouch.press.isPressed
        //     && EventSystem.current.IsPointerOverGameObject(Touchscreen.current.primaryTouch.touchId.ReadValue()))
        //     return;

        Vector2 delta = pointerDelta.ReadValue<Vector2>();

        Vector3 move = new Vector3(
            -delta.x * panSpeed,
            -delta.y * panSpeed,
            0f
        );

        transform.Translate(move, Space.Self);
    }

    void HandleVertical()
    {
        if(Keyboard.current.qKey.isPressed)
            transform.position += Vector3.up * heightSpeed * Time.deltaTime;
        
        if(Keyboard.current.eKey.isPressed)
            transform.position += Vector3.down * heightSpeed * Time.deltaTime;
    }

    void HandleRotation()
    {
        if(!rotatePress.IsPressed() || !IsRotateGestureActive())
            return;

        Vector2 delta = rotate.ReadValue<Vector2>();

        if(delta.sqrMagnitude < 0.001f)
            return;

        transform.Rotate(
            Vector3.up,
            delta.x * rotationSpeed,
            Space.World
            );

        float factor = Touchscreen.current != null ?0.5f :1f;
        float sensitivity = GetRotationSensitivity();

        float targetYaw = transform.eulerAngles.y + delta.x * rotationSpeed * sensitivity;
        float targetPitch = pitch - delta.y * rotationSpeed * sensitivity;

        float ySign = invertY ?1 :-1;

        targetPitch = pitch + ySign * delta.y * rotationSpeed;
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);

        if(useRotationSmoothing)
        {
            float smoothYaw = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetYaw,
                ref yawVelocity,
                rotationSmoothTime
            );

            float smoothPitch = Mathf.SmoothDampAngle(
                pitch,
                targetPitch,
                ref pitchVelocity,
                rotationSmoothTime
            );

            transform.rotation = Quaternion.Euler(0f, smoothYaw, 0f);
            cameraPivot.localRotation = Quaternion.Euler(smoothPitch, 0f, 0f);

            pitch = smoothPitch;
            Debug.DrawRay(cameraPivot.position, cameraPivot.forward * 2f, Color.green);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, targetYaw, 0f);
            cameraPivot.localRotation = Quaternion.Euler(targetPitch, 0f, 0f);

            pitch = targetPitch;
        }

    }

    void HandleElevation()
    {
        if(elevate == null)
            return;
        
        float v = elevate.ReadValue<float>();

        if(Mathf.Abs(v) < 0.01f)
            return;
        
        transform.position += Vector3.up * v * elevateSpeed * Time.deltaTime;
    }

    public void OnGameStateChange(GameStateType state)
    {
        Debug.Log("On Game State Change(from camera): " + state);
        switch(state)
        {
            case GameStateType.Playing:
                pointerDelta.Enable();
                pointerPress.Enable();
                zoom.Enable();
                move.Enable();
                rotate.Enable();
                rotatePress.Enable();
                elevate.Enable();
                break;

            default:
                pointerDelta.Disable();
                pointerPress.Disable();
                zoom.Disable();
                move.Disable();
                rotate.Disable();
                rotatePress.Disable();
                elevate.Disable();
                break;
        }
    }

    bool IsRotateGestureActive()
    {
        if(rotatePress != null && rotatePress.IsPressed())
            return true;
        
        if(Touchscreen.current == null)
            return false;
        
        int pressedTouches = 0;

        foreach(var touch in Touchscreen.current.touches)
        {
            if(touch.press.isPressed)
                pressedTouches++;
        }

        return pressedTouches >= 2;
    }

    float GetRotationSensitivity()
    {
        if(!useSensitivityScaling)
            return 1f;
        
        // Touch devices need lower sensitivity
        if(Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            return touchSensitivity;
        
        return mouseSensitivity;
    }
}

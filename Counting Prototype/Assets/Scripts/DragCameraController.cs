using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DragCameraController : MonoBehaviour
{
    public GameObject pachinkoMachine;

    [Header("References")]
    public Camera cam;

    [Header("Movement")]
    public float panSpeed = 0.01f;
    public float zoomSpeed = 5f;
    public float minZoom = 3f;
    public float maxZoom = 25f;

    public float heightSpeed = 5f;

    InputAction pointerDelta;
    InputAction pointerPress;
    InputAction zoom;

    void Awake()
    {

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var machine = pachinkoMachine.GetComponent<PachinkoMachineManager>();
        var playerInput = machine.GetPlayerInput();
        pointerDelta = playerInput.InGame.PointerDelta;
        pointerPress = playerInput.InGame.PointerPress;
        zoom = playerInput.InGame.Zoom;
    }

    // Update is called once per frame
    void Update()
    {
        HandlePan();
        HandleZoom();
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
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if(!pointerPress.IsPressed())
            return;

        if (
            Touchscreen.current != null
            && Touchscreen.current.primaryTouch.press.isPressed
            && EventSystem.current.IsPointerOverGameObject(Touchscreen.current.primaryTouch.touchId.ReadValue()))
            return;

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

    public void OnGameStateChange(GameStateType state)
    {
        Debug.Log("On Game State Change(from camera): " + state);
        switch(state)
        {
            case GameStateType.Playing:
                pointerDelta.Enable();
                pointerPress.Enable();
                zoom.Enable();
                break;

            default:
                pointerDelta.Disable();
                pointerPress.Disable();
                zoom.Disable();
                break;
        }
    }

}

using Unity.Cinemachine;
using UnityEngine;

public class CinemachineSwitcher : MonoBehaviour
{
    [Tooltip("Order matters: index 0 is your default camera.")]
    public CinemachineCamera[] cameras;

    [Tooltip("Input key to switch. Replace with your button/input system as needed.")]
    private KeyCode switchKey = KeyCode.C;
    private int currentIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Ensure only the first camera is active by priority
        for (int i = 0; i < cameras.Length; i++)
            cameras[i].Priority = (i == currentIndex) ? 10 : 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            currentIndex = (currentIndex + 1) % cameras.Length;
            for(int i = 0; i < cameras.Length; i++)
                cameras[i].Priority = (i == currentIndex) ?10 :0;
        }
    }
}

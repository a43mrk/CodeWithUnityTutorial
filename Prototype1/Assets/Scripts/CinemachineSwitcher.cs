using Unity.Cinemachine;
using UnityEngine;

public class CinemachineSwitcher : MonoBehaviour
{
    public CinemachineCamera cam1;
    public CinemachineCamera cam2;
    private bool usingCam1 = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            usingCam1 = !usingCam1;
            cam1.Priority = usingCam1 ?10 :0;
            cam2.Priority = usingCam1 ?0 :10;
        }
    }
}

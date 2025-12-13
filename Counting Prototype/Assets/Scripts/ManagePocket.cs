using UnityEngine;

public class ManagePocket : MonoBehaviour
{
    public GameObject leftArm;
    public GameObject rightArm;
    public float openAngle = 45.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leftArm.transform.Rotate(Vector3.back * 5);
        rightArm.transform.Rotate(Vector3.forward * 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenArms()
    {
        leftArm.transform.Rotate(Vector3.back * openAngle);
        rightArm.transform.Rotate(Vector3.forward * openAngle);
    }
}

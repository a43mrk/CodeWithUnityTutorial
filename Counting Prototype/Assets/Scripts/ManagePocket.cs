using UnityEngine;

public class ManagePocket : MonoBehaviour
{
    public GameObject leftArm;
    public GameObject rightArm;
    public float openAngle = 45.0f;
    private GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leftArm.transform.Rotate(Vector3.back * 5);
        rightArm.transform.Rotate(Vector3.forward * 5);

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.IsJackpotTime())
        {
            OpenArms();
        }
    }

    public void OpenArms()
    {
        leftArm.transform.Rotate(Vector3.back * openAngle);
        rightArm.transform.Rotate(Vector3.forward * openAngle);
    }

    public void CloseArms()
    {
        leftArm.transform.Rotate(Vector3.forward * openAngle);
        rightArm.transform.Rotate(Vector3.back * openAngle);
    }
}

using UnityEngine;

public class ManagePocket : MonoBehaviour
{
    public GameObject leftArm;
    public GameObject rightArm;
    public float openAngle = 45.0f;
    private GameManager gameManager;
    private bool isClosed = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leftArm.transform.Rotate(Vector3.down * 5);
        rightArm.transform.Rotate(Vector3.up * 5);

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.IsJackpotTime())
        {
            if(isClosed)
                OpenArms();
        }
    }

    public void OpenArms()
    {
        isClosed = false;
        leftArm.transform.Rotate(Vector3.down * openAngle);
        rightArm.transform.Rotate(Vector3.up * openAngle);
    }

    public void CloseArms()
    {
        isClosed = true;
        leftArm.transform.Rotate(Vector3.up * openAngle);
        rightArm.transform.Rotate(Vector3.down * openAngle);
    }
}

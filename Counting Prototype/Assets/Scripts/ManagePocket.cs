using UnityEngine;

public class ManagePocket : MonoBehaviour
{
    public GameObject leftArm;
    public GameObject rightArm;
    public float openAngle = 45.0f;
    private GameManager gameManager;
    private bool isClosed = true;
    private Animator leftArmAnimator;
    private Animator rightArmAnimator;
    private static readonly int IsOpenHash = Animator.StringToHash("IsOpen");

    void Awake()
    {
        leftArmAnimator = leftArm.GetComponent<Animator>();
        rightArmAnimator = rightArm.GetComponent<Animator>();

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        leftArm.transform.Rotate(Vector3.down * 5);
        rightArm.transform.Rotate(Vector3.up * 5);
    }

    public void OpenArms()
    {
        // isClosed = false;
        // leftArm.transform.Rotate(Vector3.down * openAngle);
        // rightArm.transform.Rotate(Vector3.up * openAngle);
        leftArmAnimator.SetBool(IsOpenHash, true);
        rightArmAnimator.SetBool(IsOpenHash, true);
    }

    public void CloseArms()
    {
        // isClosed = true;
        // leftArm.transform.Rotate(Vector3.up * openAngle);
        // rightArm.transform.Rotate(Vector3.down * openAngle);
        leftArmAnimator.SetBool(IsOpenHash, false);
        rightArmAnimator.SetBool(IsOpenHash, false);
    }

    public bool IsOpen() => leftArmAnimator.GetBool(IsOpenHash) && rightArmAnimator.GetBool(IsOpenHash);
}

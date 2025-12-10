using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -7);


    // LateUpdate is good for cameras
    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    [SerializeField] private int capacity = 500;
    [SerializeField] private Transform dropPoint;

    private readonly List<GameObject> balls = new();
    public bool IsFull => balls.Count >= capacity;

    public bool TryAddBall(GameObject ball)
    {
        if(IsFull)
            return false;

        var rb = ball.GetComponent<Rigidbody>();

        if(!rb)
            return false;

        balls.Add(ball);

        ball.transform.SetParent(transform);

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Drop into center
        ball.transform.position = dropPoint.position;

        return true;
    }
}

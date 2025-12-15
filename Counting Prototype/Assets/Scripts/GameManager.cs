using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject ballPrefab;
    public GameObject startPointAndDirection;
    public int startCredits = 100;
    public int interval = 1;
    public float initialForce = 300f;
    public float maxForce = 500f;

    [UnitHeaderInspectable("Gravity Settings")]
    public Vector3 gravity = new Vector3(0f, -9.81f, 0f);
    public Text CounterText;

    private int totalScore = 0;

    void Awake()
    {
        // Apply global gravity at startup
        Physics.gravity = gravity;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnAndShoot());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator SpawnAndShoot()
    {
        while(startCredits >0)
        {
            ShootBall();
            startCredits--;
            yield return new WaitForSeconds(interval);
        }
    }

    private void ShootBall()
    {
        GameObject ball = Instantiate(
            ballPrefab,
            startPointAndDirection.transform.position,
            Quaternion.identity
        );

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if(rb == null)
        {
            Debug.LogError("Ball prefab must have a Rigidbody.");
            return;
        }

        Vector3 direction = startPointAndDirection.transform.up;
        rb.AddForce(direction * UnityEngine.Random.Range(initialForce, maxForce), ForceMode.Impulse);
    }

    public void UpdateScore(int points)
    {
        totalScore += points;
        CounterText.text = "Count : " + totalScore;
    }
}

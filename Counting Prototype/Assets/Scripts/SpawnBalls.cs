using System.Collections;
using UnityEngine;

public class SpawnBalls : MonoBehaviour
{
    public GameObject ballPrefab;
    public int ballNo = 0;
    public int maxBalls = 600;
    private Coroutine routine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        routine = StartCoroutine(SpawnBall());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator SpawnBall()
    {

        while(ballNo <= maxBalls)
        {
            ballNo++;
            Instantiate(ballPrefab, transform.position, transform.rotation);

            yield return null;

            if(ballNo >= maxBalls)
            {
                StopCoroutine(routine);
                routine = null;
            }
        }
    }
}

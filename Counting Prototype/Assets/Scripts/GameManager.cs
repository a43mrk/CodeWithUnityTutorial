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
    public Text LostBallsText;

    private int totalScore = 0;

    private int ballsLost = 0;
    // Payout amount is fixed (e.g., 10, 15, or 20 balls).
    public int jackPotPremium = 15; // normally 15
    public int sidePocketPremium = 3; // 0 ~ 3 balls
    public float jackPointTime = 30.0f; // 30~60 seconds
    public int jackpotRounds = 8; // 8~15 rounds
    private float jackPointTimeLeft = 0;
    private int ballsMissed = 0;
    private int redeemedMissedBalls = 0;
    // used to toggle winning light when users redeem the balls
    private bool isWinningBalls = false;
    public bool isJackpotTimeBased = true; // in case of false it will use round based jackpot system.
    public int foulBallCount = 0; // Foul ball pockets: These collect balls that don’t count toward scoring but may accumulate until released.
    private AudioSource shootingAudioFx;
    public Animator leverAnimator;

    void Awake()
    {
        // Apply global gravity at startup
        Physics.gravity = gravity;
        shootingAudioFx = GetComponent<AudioSource>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnAndShoot());
    }

    // Update is called once per frame
    void Update()
    {
        if(IsJackpotTime())
        {
            UpdateJackPockCountdown();
        }
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
        shootingAudioFx.Play();
        leverAnimator.SetTrigger("Pull"); // play lever animation

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

    public void IncrementBallsLost()
    {
        ++ballsLost;
        LostBallsText.text = $"Lost: {ballsLost}";
    }

    public void PushJackPocketTime()
    {
        jackPointTimeLeft += jackPointTime;
    }

    private void UpdateJackPockCountdown()
    {
        jackPointTimeLeft -= Time.deltaTime;
    }
    public bool IsJackpotTime()
    {
        return jackPointTimeLeft > 0.01f;
    }
}

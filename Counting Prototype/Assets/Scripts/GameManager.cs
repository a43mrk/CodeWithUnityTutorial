using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameDifficulty
{
    Easy,
    Normal,
    Hard
}

public enum GameActionType
{
    ChooseDificulty,
    Pause,
    Resume,
    Restart,
    ExitToMenu,
    QuitGame,
    StartGame
}

public enum GameStateType
{
    Waiting,
    Playing,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public GameObject ballPrefab;
    public GameObject startPointAndDirection;
    public GameObject jackpotStartPointAndDirection;
    public int startCredits = 100;
    public int jackpotReserve = 200;
    public int interval = 1;
    public float initialForce = 300f;
    public float maxForce = 500f;

    [UnitHeaderInspectable("Gravity Settings")]
    public Vector3 gravity = new Vector3(0f, -9.81f, 0f);

    private int totalScore = 0;

    private int ballsLost = 0;
    // Payout amount is fixed (e.g., 10, 15, or 20 balls).
    // public int jackPotPremium = 15; // normally 15
    // public int sidePocketPremium = 3; // 0 ~ 3 balls
    // public int jackpotRounds = 8; // 8~15 rounds
    public float jackPointTime = 30.0f; // 30~60 seconds
    private float jackPointTimeLeft = 0;

    private int ballsMissed = 0;
    private int redeemedMissedBalls = 0;
    // used to toggle winning light when users redeem the balls
    private bool isWinningBalls = false;
    public bool isJackpotTimeBased = true; // in case of false it will use round based jackpot system.
    public int foulBallCount = 0; // Foul ball pockets: These collect balls that don’t count toward scoring but may accumulate until released.
    private AudioSource shootingAudioFx;
    public Animator leverAnimator;
    private GameObject[] allTulips;
    public GameObject QueenLamp;
    public GameObject KingLamp;
    public GameObject ShootingChamberLamp;
    public GameObject victoryLamp;
    public GameObject victorySign;

    public GameActionChannel gameActionChannel;
    public GameDifficultyChannel gameDifficultyChannel;
    public GameStateChannel gameStateChannel;

    private GlowingLamp queensLamp;
    private GlowingLamp kingsLamp;
    private UIManager uiManager;
    private GameDifficulty gameDifficulty;
    private bool isGameRunning = false;
    public GameStateType State { get; private set; }

    private Coroutine Shooter;
    private bool isAutoShoot = false;


    void Awake()
    {
        // Apply global gravity at startup
        Physics.gravity = gravity;
        shootingAudioFx = GetComponent<AudioSource>();
        allTulips = GameObject.FindGameObjectsWithTag("Tulip");
        queensLamp = QueenLamp.GetComponent<GlowingLamp>();
        kingsLamp = KingLamp.GetComponent<GlowingLamp>();
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        State = GameStateType.Waiting;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckCredits();
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

            CheckCredits();
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
        uiManager.SetScore(points);
    }

    public void IncrementBallsLost()
    {
        ++ballsLost;
        uiManager.SetBallsLost(ballsLost);
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

    public void ExecutePayout(int prize)
    {
        kingsLamp.EnableGlow();

        if(jackpotReserve > 0)
        {
            jackpotReserve -= prize;

            for(int i = 0; i < prize; i++)
            {
                GameObject ball = Instantiate(
                    ballPrefab,
                    jackpotStartPointAndDirection.transform.position,
                    Quaternion.identity
                );

                // Debug.Log("instantiating payout ball: " + ball.gameObject.GetInstanceID());

                Rigidbody rb = ball.GetComponent<Rigidbody>();
                if(rb == null)
                {
                    Debug.LogError("Ball prefab must have a Rigidbody.");
                    return;
                }

                Vector3 direction = jackpotStartPointAndDirection.transform.up;
                rb.AddForce(direction * 50, ForceMode.Impulse);
            }
        }
        else
        {
            queensLamp.EnableGlow();
            victoryLamp.GetComponent<GlowingLamp>()?.EnableGlow();
            victorySign.SetActive(true);
        }

    }

    public void OpenAllTulips()
    {
        foreach(var tulip in allTulips.Select(i => i.GetComponent<ManagePocket>()))
        {
            tulip.OpenArms();
        }
    }

    public void CloseAllTulips()
    {
        foreach(var tulip in allTulips.Select(i => i.GetComponent<ManagePocket>()))
        {
            tulip.CloseArms();
        }
    }

    public void SetGameDifficutyToEasy()
    {
        gameDifficulty = GameDifficulty.Easy;
        gameDifficultyChannel.Invoke(gameDifficulty);

        StartGame();
    }
    public void SetGameDifficutyToNormal()
    {
        gameDifficulty = GameDifficulty.Normal;
        gameDifficultyChannel.Invoke(gameDifficulty);

        StartGame();
    }
    public void SetGameDifficutyToHard()
    {
        gameDifficulty = GameDifficulty.Hard;
        gameDifficultyChannel.Invoke(gameDifficulty);

        StartGame();
    }

    public void Play()
    {
        gameActionChannel.Invoke(GameActionType.ChooseDificulty);
    }

    public void StartGame()
    {
        State = GameStateType.Playing;
        if(Shooter == null && isAutoShoot)
            Shooter = StartCoroutine(SpawnAndShoot());

        gameStateChannel.Invoke(GameStateType.Playing);
    }

    public void RestartGame()
    {
        gameActionChannel.Invoke(GameActionType.Restart);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
        gameActionChannel.Invoke(GameActionType.Pause);
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        gameActionChannel.Invoke(GameActionType.Resume);
    }

    public GameDifficulty GetGameDifficulty() => gameDifficulty;

    private void CheckCredits()
    {
        if (startCredits <= 0)
        {
            queensLamp.EnableGlow();
        }
    }

    public bool ToggleAutoShoot()
    {
        isAutoShoot = !isAutoShoot;

        return isAutoShoot;
    }

    public void SetAutoShoot(bool val)
    {
        Debug.Log("SetAutoShoot: " + val);
        isAutoShoot = val;

        if(Shooter == null && isAutoShoot)
        {
            Shooter = StartCoroutine(SpawnAndShoot());
        }
        else if(Shooter != null && !isAutoShoot)
        {
            StopCoroutine(Shooter);
        }
    }

    public bool IsAutoShoot() => isAutoShoot;
}

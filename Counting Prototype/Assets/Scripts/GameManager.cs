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
    GoToSettingsMenu,
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

public enum ShootingType
{
    ManualOrAutomatic,
    SemiAutomatic
}


public class GameManager : MonoBehaviour
{
    public GameObject machine;
    [SerializeField] private int startCredits = 100;


    [Header("Gravity Settings")]
    public Vector3 gravity = new Vector3(0f, -9.81f, 0f);

    private int totalScore = 0;
    private int ballsLost = 0;
    private int missedShoots = 0;
    private int totalCollectedBalls = 0;
    private int ballsReused = 0;
    private int missingBalls = 0;

    // used to toggle winning light when users redeem the balls
    public bool isJackpotTimeBased = true; // in case of false it will use round based jackpot system.

    public GameActionChannel gameActionChannel;
    public GameDifficultyChannel gameDifficultyChannel;
    public GameStateChannel gameStateChannel;

    private UIManager uiManager;
    private GameDifficulty gameDifficulty;
    private bool isGameRunning = false;

    public GameStateType State { get; private set; }

    public ShootingType shootingType = ShootingType.SemiAutomatic;



    void Awake()
    {
        // Apply global gravity at startup
        Physics.gravity = gravity;
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        State = GameStateType.Waiting;
    }

    public int TakeABall()
    {
        uiManager.SetShootingBalls(--startCredits);
        return startCredits;
    }

    public int GetAvailableBalls() => startCredits;

    public int IncrementBall(bool isReused = false)
    {
        if(isReused)
            ++ballsReused;

        uiManager.SetShootingBalls(++startCredits);
        return startCredits;
    }

    public void UpdateScore(int points)
    {
        totalScore += points;
        uiManager.SetScore(totalScore);
    }

    public void IncrementBallsLost()
    {
        ++ballsLost;
        uiManager.SetBallsLost(ballsLost);
    }

    public void IncrementMissedShoots()
    {
        ++missedShoots;
        uiManager.SetMissedShoots(missedShoots);
    }

    public void IncrementMissingBalls()
    {
        ++missingBalls;
        uiManager.SetMissingBalls(missingBalls);
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

    public void ChooseLanguage()
    {
        gameActionChannel.Invoke(GameActionType.GoToSettingsMenu);
    }

    public void Play()
    {
        gameActionChannel.Invoke(GameActionType.ChooseDificulty);
    }

    public void StartGame()
    {
        State = GameStateType.Playing;

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

    public void SetManualOrAutomaticShootingMethod(bool really)
    {
        if(really)
        {
            shootingType = ShootingType.ManualOrAutomatic;
        }
        else
        {
            shootingType = ShootingType.SemiAutomatic;
        }
    }

    public void SetSemiAutomaticShootingMethod(bool really)
    {
        if(really)
        {
            shootingType = ShootingType.SemiAutomatic;
        }
        else
        {
            shootingType = ShootingType.ManualOrAutomatic;
        }
    }

    public GameDifficulty GetGameDifficulty() => gameDifficulty;

    public PachinkoMachineManager GetMachine() => machine.GetComponent<PachinkoMachineManager>();
}

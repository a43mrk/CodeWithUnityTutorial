using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject difficultyMenu;
    public GameObject gamePanel;
    public GameObject playBtn;
    public GameObject restartBtn;
    public GameObject resumeBtn;

    public Text CounterText;
    public Text LostBallsText;
    private GameActionType lastAction;

    void Start()
    {
        difficultyMenu.SetActive(false);
        restartBtn.SetActive(false);
        resumeBtn.SetActive(false);
        gamePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGameActionChanges(GameActionType actionType)
    {

        Debug.Log("Action Type: " + actionType);
        switch(actionType)
        {
            case GameActionType.ChooseDificulty:
                playBtn.SetActive(false);
                difficultyMenu.SetActive(true);
                break;
            case GameActionType.Pause:
                resumeBtn.SetActive(true);
                restartBtn.SetActive(true);
                break;
            case GameActionType.Restart:
                resumeBtn.SetActive(false);
                restartBtn.SetActive(false);
                break;
            case GameActionType.Resume:
                resumeBtn.SetActive(false);
                restartBtn.SetActive(false);
                break;
        }

        lastAction = actionType;
    }
    public void OnGameDifficultyChange(GameDifficulty difficulty)
    {
        Debug.Log("difficulty: " + difficulty);
        difficultyMenu.SetActive(false);
        gamePanel.SetActive(true);
        // Start Game
    }

    public void SetBallsLost(int ballsLost)
    {
        LostBallsText.text = $"Lost: {ballsLost}";
    }

    public void SetScore(int points)
    {
        CounterText.text = "Count : " + points;
    }
}

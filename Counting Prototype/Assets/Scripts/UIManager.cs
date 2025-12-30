using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject difficultyMenu;
    public GameObject gamePanel;
    public GameObject gameControlOptions;

    public Text CounterText;
    public Text LostBallsText;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGameActionChanges(GameActionType actionType)
    {
        Debug.Log("Action Type: " + actionType);
    }
    public void OnGameDifficultyChange(GameDifficulty difficulty)
    {
        Debug.Log("difficulty: " + difficulty);
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

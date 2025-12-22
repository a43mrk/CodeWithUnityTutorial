using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TulipsThatCanOpen
{
    None,
    OnList,
    All
}

public class Counter : MonoBehaviour
{

    public int pointsWorth = 1;
    private GameManager gameManager;
    // flag this pocket as jackpoint counter
    public bool isJackpotCollector = false;
    public bool isFoulBallCollector = false;
    public TulipsThatCanOpen openTulipsType = TulipsThatCanOpen.None;

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        // cannot be both foulballCollector and jackpotcollector
        if(isFoulBallCollector)
        {
            isJackpotCollector = false;
        } else if(isJackpotCollector)
        {
            isFoulBallCollector = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
        gameManager.UpdateScore(pointsWorth);
        if(isJackpotCollector)
        {
            gameManager.PushJackPocketTime();
        }
    }
}

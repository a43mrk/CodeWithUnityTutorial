using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum TulipsThatCanOpen
{
    None,
    Itself,
    ItselfAndOnList,
    OnList,
    All
}
public enum TulipCloseBehaviorType
{
    None,
    Itself,
    ItselfAndOnList,
    OnList,
    All
}

public class Counter : MonoBehaviour
{

    public int pointsWorth = 1;
    // flag this pocket as jackpoint counter
    public TulipsThatCanOpen openTulipsType = TulipsThatCanOpen.None;
    public List<GameObject> connectedTulips = new List<GameObject>();
    public TulipCloseBehaviorType tulipCloseBehavior = TulipCloseBehaviorType.None;
    public Animator starAnimator;
    public int count = 0;

    private GameManager gameManager;
    private ManagePocket tulip;

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        tulip = GetComponent<ManagePocket>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Ball")) return;

        Destroy(other.gameObject);
        ++count;
        gameManager.UpdateScore(pointsWorth);

        ProcessPocket();

        if(starAnimator != null)
        {
            starAnimator.SetTrigger("Pull");
        }

    }

    void ProcessPocket()
    {
        if(tulip == null)
        {
            ProcessPocketForNonTulips();
            return;
        }

        if(!tulip.IsOpen())
        {
            switch(openTulipsType)
            {
                case TulipsThatCanOpen.Itself:
                    tulip.OpenArms();
                    break;
                case TulipsThatCanOpen.ItselfAndOnList:
                    tulip.OpenArms();
                    connectedTulips.ForEach(i =>
                    {
                        i.GetComponent<ManagePocket>()?.OpenArms();
                    });
                    break;

                case TulipsThatCanOpen.OnList:
                    connectedTulips.ForEach(i =>
                    {
                        i.GetComponent<ManagePocket>()?.OpenArms();
                    });
                    break;
                case TulipsThatCanOpen.All:
                    gameManager.OpenAllTulips();
                    break;
                default:
                    break;
            }

        } else {
            switch(tulipCloseBehavior)
            {
                case TulipCloseBehaviorType.Itself:
                    tulip.CloseArms();
                    break;
                case TulipCloseBehaviorType.ItselfAndOnList:
                    tulip.CloseArms();
                    connectedTulips.ForEach(i =>
                    {
                        i.GetComponent<ManagePocket>()?.CloseArms();
                    });
                    break;

                case TulipCloseBehaviorType.OnList:
                    connectedTulips.ForEach(i =>
                    {
                        i.GetComponent<ManagePocket>()?.CloseArms();
                    });
                    break;
                case TulipCloseBehaviorType.All:
                    gameManager.CloseAllTulips();
                    break;
                default:
                    break;
            }

        }

        // Pays the prize
        gameManager.ExecutePayout();
    }

    void ProcessPocketForNonTulips()
    {
            switch(openTulipsType)
            {
                case TulipsThatCanOpen.OnList:
                    connectedTulips.ForEach(i =>
                    {
                        i.GetComponent<ManagePocket>()?.OpenArms();
                    });
                    break;
                case TulipsThatCanOpen.All:
                    gameManager.OpenAllTulips();
                    break;
                default:
                    break;
            }
            switch(tulipCloseBehavior)
            {
                case TulipCloseBehaviorType.OnList:
                    connectedTulips.ForEach(i =>
                    {
                        i.GetComponent<ManagePocket>()?.CloseArms();
                    });
                    break;
                case TulipCloseBehaviorType.All:
                    gameManager.CloseAllTulips();
                    break;
                default:
                    break;
            }


        // Pays the prize
        gameManager.ExecutePayout();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public int prize = 15;
    // flag this pocket as jackpoint counter
    public TulipsThatCanOpen openTulipsType = TulipsThatCanOpen.None;
    public List<GameObject> connectedTulips = new List<GameObject>();
    public TulipCloseBehaviorType tulipCloseBehavior = TulipCloseBehaviorType.None;
    public Animator starAnimator;
    public int count = 0;

    private GameManager gameManager;
    private PachinkoMachineManager pachinkoMachine;
    private ManagePocket tulip;
    public GameObject indicatorLamp;
    public GameObject indicatorLamp2;
    public AudioClip[] tracks;
    private GlowingLamp lamp;
    private GlowingLamp lamp2;
    private AudioSource audioSource;

    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        pachinkoMachine = gameManager.GetMachine();
        tulip = GetComponent<ManagePocket>();

        if(lamp != null)
            lamp = indicatorLamp.GetComponent<GlowingLamp>();

        if(indicatorLamp2 != null)
            lamp2 = indicatorLamp2.GetComponent<GlowingLamp>();

        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.CompareTag("Ball")) return;

        Destroy(other.gameObject);
        ++count;
        gameManager.UpdateScore(pointsWorth);

        ProcessPocket();

        if(lamp != null)
        {
            lamp.StartLampSequence();
            // lamp.StartPulsatingLampSequence();
        }

        if(lamp2 != null)
        {
            lamp2.StartLampSequence();
            // lamp2.StartPulsatingLampSequence();
        }

        if(starAnimator != null)
        {
            starAnimator.SetTrigger("Pull");
        }

        if(audioSource != null)
        {
            if(audioSource.isPlaying)
                audioSource.Stop();

            foreach(var track in tracks)
            {
                audioSource.PlayOneShot(track);
            }

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
                    pachinkoMachine.OpenAllTulips();
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
                    pachinkoMachine.CloseAllTulips();
                    break;
                default:
                    break;
            }

        }

        // Pays the prize
        pachinkoMachine.ExecutePayout(prize);
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
                    pachinkoMachine.OpenAllTulips();
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
                    pachinkoMachine.CloseAllTulips();
                    break;
                default:
                    break;
            }


        // Pays the prize
        pachinkoMachine.ExecutePayout(prize);
    }
}

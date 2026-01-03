using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PachinkoMachineManager : MonoBehaviour
{
    public GameObject ballCollector;
    GameManager gameManager;
    RechargeOrCollect whiteTray;
    private Player1 player1actions;

    void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        player1actions = new Player1();
        whiteTray = ballCollector.GetComponent<RechargeOrCollect>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupActionMap();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        player1actions.OnMenu.EscapeMenu.performed += OnEscapeMenu;
        player1actions.InGame.OpenMainMenu.performed += OnOpenMainMenu;
        player1actions.InGame.CollectBall.performed += whiteTray.OnCollectAction;
        player1actions.InGame.UseBall.performed += OnUseBall;
    }

    public Player1 GetPlayerInput() => player1actions;

    private void OnUseBall(InputAction.CallbackContext context)
    {
        var ball = whiteTray.TakeABall();
        if(ball != null)
        {
            Destroy(ball);
            ++gameManager.startCredits;
            Debug.Log("Adding ball to credits");
        }
    }

    void OnDisable()
    {
        player1actions.OnMenu.EscapeMenu.performed -= OnEscapeMenu;
        player1actions.InGame.OpenMainMenu.performed -= OnOpenMainMenu;
        player1actions.InGame.CollectBall.performed -= whiteTray.OnCollectAction;
        player1actions.InGame.UseBall.performed -= OnUseBall;
    }

    private void OnOpenMainMenu(InputAction.CallbackContext context)
    {
        Debug.Log("Open Main Menu triggered");
        // TODO: Pause game -> show the Main Menu
        gameManager.PauseGame();
    }

    private void OnEscapeMenu(InputAction.CallbackContext context)
    {
        Debug.Log("Escape Menu triggered");
        // TODO: make it back the previous menu or exit main menu if on the root
    }


    public void SetupActionMap()
    {
        if (gameManager.State == GameStateType.Waiting)
        {
            player1actions.InGame.Disable();
            player1actions.OnMenu.Enable();
        }
        else if (gameManager.State == GameStateType.Playing)
        {
            player1actions.OnMenu.Disable();
            player1actions.InGame.Enable();
        }
    }

    public void OnGameStateChange(GameStateType state)
    {
        switch(state)
        {
            case GameStateType.Waiting:
                player1actions.InGame.Disable();
                player1actions.OnMenu.Enable();
                break;

            case GameStateType.Playing:
                player1actions.OnMenu.Disable();
                player1actions.InGame.Enable();
                break;
        }
    }
}

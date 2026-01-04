using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PachinkoMachineManager : MonoBehaviour
{
    public GameObject ballPrefab;
    public GameObject startPointAndDirection;
    public GameObject jackpotStartPointAndDirection;
    public int jackpotReserve = 200;
    public int interval = 1;
    public float initialForce = 300f;
    public float maxForce = 500f;
    [SerializeField]
    private float maxHoldTime = 2f;
    [SerializeField]
    private AnimationCurve skillCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public GameObject ballCollector;

    [Header("Hold Action Settings")]
    [SerializeField] private float repeatInterval = 0.1f;


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
    [SerializeField] private Transform leverTransform;

    [Header("Button References")]
    public Animator orangeBtnAnimator;
    [SerializeField] private Transform orangeBtnTransform;

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



    GameManager gameManager;
    RechargeOrCollect whiteTray;
    private Player1 player1actions;

    private float holdTimer;
    private bool isHoldingShootingLever;
    private bool holdQualified;

    private bool isHoldingOrangeBtn;

    private Coroutine orangeBtnHoldCoroutine;
    public Camera mainCamera;
    private static readonly int IsPressedHash = Animator.StringToHash("IsPressed");

    void Awake()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        player1actions = new Player1();
        whiteTray = ballCollector.GetComponent<RechargeOrCollect>();

        shootingAudioFx = GetComponent<AudioSource>();
        allTulips = GameObject.FindGameObjectsWithTag("Tulip");
        queensLamp = QueenLamp.GetComponent<GlowingLamp>();
        kingsLamp = KingLamp.GetComponent<GlowingLamp>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupActionMap();
        CheckCredits();
    }


    // Update is called once per frame
    void Update()
    {
        // if(IsJackpotTime())
        // {
        //     UpdateJackPockCountdown();
        // }

        if(!isHoldingShootingLever)
            return;
        
        holdTimer += Time.deltaTime;
        holdTimer = Mathf.Min(holdTimer, maxHoldTime);

        float normalized = holdTimer / maxHoldTime;
        leverAnimator.SetFloat("PullAmount", normalized);


    }

    void OnEnable()
    {
        player1actions.OnMenu.EscapeMenu.performed += OnEscapeMenu;
        player1actions.InGame.OpenMainMenu.performed += OnOpenMainMenu;
        player1actions.InGame.CollectBall.performed += whiteTray.OnCollectAction;
        player1actions.InGame.UseBall.performed += OnUseBall;
        player1actions.InGame.PullLever.started += OnPullStarted;
        player1actions.InGame.PullLever.performed += OnPullPerformed;
        player1actions.InGame.PullLever.canceled += OnPullCanceled;
        player1actions.InGame.PressOrangeButton.started += OnPressStarted;
        player1actions.InGame.PressOrangeButton.canceled += OnPressCanceled;
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
        player1actions.InGame.PullLever.started -= OnPullStarted;
        player1actions.InGame.PullLever.performed -= OnPullPerformed;
        player1actions.InGame.PullLever.canceled -= OnPullCanceled;
        player1actions.InGame.PressOrangeButton.started -= OnPressStarted;
        player1actions.InGame.PressOrangeButton.canceled -= OnPressCanceled;
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
                StartGame();
                break;
        }
    }


    public void StartGame()
    {
        if(Shooter == null && isAutoShoot)
            Shooter = StartCoroutine(SpawnAndShoot());
    }

    private IEnumerator SpawnAndShoot()
    {
        while(gameManager.startCredits >0)
        {
            ShootBall();
            gameManager.startCredits--;
            yield return new WaitForSeconds(interval);

            CheckCredits();
        }
    }

    public void ShootBall()
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

    private void CheckCredits()
    {
        if (gameManager.startCredits <= 0)
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

    private void OnPullCanceled(InputAction.CallbackContext context)
    {
        if(!holdQualified)
        {
            ResetLever();
            return;
        }

        FireLever();
        ResetLever();
    }

    private void FireLever()
    {
        float normalized = Mathf.Clamp01(holdTimer / maxHoldTime);
        float force = skillCurve.Evaluate(normalized) * maxForce;


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
        rb.AddForce(direction * force, ForceMode.Impulse);

        leverAnimator.SetTrigger("Shoot");
    }

    private void ResetLever()
    {
        isHoldingShootingLever = false;
        holdTimer = 0f;

        leverAnimator.SetBool("Holding", false);
        leverAnimator.SetFloat("PullAmount", 0f);
    }

    private void OnPullPerformed(InputAction.CallbackContext context)
    {
        // Hold duration reached (long press confirmed)
        holdQualified = true;
    }

    private void OnPullStarted(InputAction.CallbackContext context)
    {

        if(!PointerHitsThisButton(context, leverTransform))
            return;

        isHoldingShootingLever = true;
        holdQualified = false;
        holdTimer = 0f;

        leverAnimator.SetBool("Holding", true);
    }

    void OnOrangeBtnPressStarted()
    {
        orangeBtnAnimator.SetBool("IsPressed", true);
    }

    void OnOrangeBtnPressCanceled()
    {
        orangeBtnAnimator.SetBool("IsPressed", false);
    }

    public void PressOrangeBtn()
    {
        orangeBtnAnimator.SetTrigger("Push");
    }


    private void OnPressCanceled(InputAction.CallbackContext context)
    {
        if (!isHoldingOrangeBtn)
            return;

        isHoldingOrangeBtn = false;
        orangeBtnAnimator.SetBool(IsPressedHash, false);

        if(orangeBtnHoldCoroutine != null)
        {
            StopCoroutine(orangeBtnHoldCoroutine);
            orangeBtnHoldCoroutine = null;
        }

        OnButtonReleased();
    }


    private void OnPressStarted(InputAction.CallbackContext context)
    {
        if(!PointerHitsThisButton(context, orangeBtnTransform))
            return;

        isHoldingOrangeBtn = true;
        orangeBtnAnimator.SetBool(IsPressedHash, true);

        orangeBtnHoldCoroutine = StartCoroutine(HoldLoop());
    }

    private IEnumerator HoldLoop()
    {
        while(isHoldingOrangeBtn)
        {
            OnButtonHeld();

            if(repeatInterval <= 0f)
                yield return null;
            else
                yield return new WaitForSeconds(repeatInterval);
        }
    }

    // ====================
    // Gameplay hooks
    // ====================
    private void OnButtonHeld()
    {
        Debug.Log("Button held");
    }
    private void OnButtonReleased()
    {
        Debug.Log("Button released");
    }

    /// <summary>
    /// HIT TEST
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private bool PointerHitsThisButton(InputAction.CallbackContext context, Transform target)
    {
        // Gamepad / keyboard: no pointer, accept input
        if(context.control.device is Gamepad || context.control.device is Keyboard)
            return true;
        
        if(!(context.control.device is Pointer))
            return false;

        Vector2 screenPos = Pointer.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(screenPos);

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.transform == target
                || hit.transform.IsChildOf(target);
        }

        return false;
    }
}

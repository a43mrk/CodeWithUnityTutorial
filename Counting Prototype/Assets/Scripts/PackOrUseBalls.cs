using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///  This is appended on Orange Btn
/// </summary>
public class PackOrUseBalls : MonoBehaviour
{
    public GameObject greenTray; // *sensor TODO: rename it
    public GameObject basquetPrefab;
    public GameObject basquetSpawnPoint;
    [SerializeField] private float repeatInterval = 0.1f;
    [SerializeField] private Camera mainCamera;


    [Header("Grid")]
    [SerializeField] private int gridWidth = 3;
    [SerializeField] private int gridDepth = 3;
    [SerializeField] private float spacingX = 1.5f;
    [SerializeField] private float spacingY = 1.5f;
    [SerializeField] private float spacingZ = 1.2f;

    private List<GameObject> baskets = new List<GameObject>();
    private Animator orangeBtnAnimator;
    private bool isHoldingOrangeBtn;

    private Coroutine orangeBtnHoldCoroutine;
    private TraySensor sensor;
    private static readonly int IsPressedHash = Animator.StringToHash("IsPressed");

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        orangeBtnAnimator = GetComponent<Animator>();
        SpawnNextBasket();
        sensor = greenTray.GetComponent<TraySensor>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddBall(GameObject ball)
    {
        foreach(var basket in baskets)
        {
            var b = basket.GetComponent<Basket>();

            if(!b.IsFull)
            {
                b.TryAddBall(ball);
                return;
            }
        }

        // All full -> spawn new
        SpawnNextBasket();
        baskets[^1].GetComponent<Basket>().TryAddBall(ball);
    }

    private void SpawnNextBasket()
    {
        int index = baskets.Count;

        int x = index & gridWidth;
        int z = (index / gridWidth) % gridDepth;
        int y = index / (gridWidth * gridDepth);

        Vector3 offset = new Vector3(
            -(x * spacingX),
            y * spacingY,
            z * spacingZ
        );

        baskets.Add(
            Instantiate(basquetPrefab, basquetSpawnPoint.transform.position + offset, basquetSpawnPoint.transform.rotation)
        );
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

    public void OnPressCanceled(InputAction.CallbackContext context)
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

    public void OnPressStarted(InputAction.CallbackContext context)
    {
        if(!PointerHitsThisButton(context, transform))
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
        Debug.Log("Button held: Adding Ball");
        AddBall( sensor.ballsInTray.FirstOrDefault());
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

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


[DefaultExecutionOrder(-1)]
public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;
    public static PlayerController Instance { get { return instance; } }

    private Rigidbody rb;
    [SerializeField] float jumpHeight, downPower;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float groundCheckOffset;

    [SerializeField] private InputActionAsset swipeInputActions;
    //private PlayerMovement playerMovement;
    public delegate void StartTouch(Vector2 pos);
    public event StartTouch onStartEvent;
    public delegate void EndTouch(Vector2 pos);

    public Action CancelledEvent;
    public event EndTouch onEndEvent;
    public float playerSpeed;
    public bool canMove, isInvincible, canSpawnCoin;
    public bool canSwipe, isSwipePerformed, isSwipeAllowed;

    [Range(1, 10)]
    [SerializeField] float rotationfactor;

    public float score, lastTrackScore, lastCoinPlaformValue;
    private int thisRunCoinValue;

    [SerializeField] GameObject Shield, thisBall;
    public ParticleSystem explosion;
    private float gravityScale = 3f;
    private static float globalGravity = -9.81f;

    public Vector3 movePos;
    Transform playerObject;
    int count = 1;

    [SerializeField] Material worldMat;
    [SerializeField] Material[] otherMat;
    private enum type { normal, neon };
    [SerializeField] private type levelType;
    bool isLeaningLeft;

    public MeshRenderer ballSkin;
    public TrailRenderer trailSkin;

    private const string leaderboard_top_score_leaderboard = "CgkIpYyxrb4UEAIQAQ";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        //playerMovement = new PlayerMovement();
        canMove = false;
        playerObject = transform.GetChild(0);
    }
    private void Start()
    {
        isSwipeAllowed = false;
        rb = GetComponent<Rigidbody>();
        thisRunCoinValue = 0;
        if (levelType == type.normal)
        {
            isLeaningLeft = true;
            foreach (var item in otherMat)
            {
                item.SetFloat("_Fwd", -0.0007f);
            }
            if (worldMat != null)
            {
                worldMat.SetFloat("_sidewaysStrength", -0.0007f);
            }
        }
        swipeInputActions.FindAction("PrimaryContact").started += StartTouchPrimary;
        swipeInputActions.FindAction("PrimaryContact").canceled += CheckSwipeInput;

        //playerMovement.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        //playerMovement.Touch.PrimaryPosition.performed += ctx => CheckSwipeInput(ctx);
        //playerMovement.Touch.PrimaryContact.canceled += ctx => CancelInput(ctx);

    }
    private void OnEnable()
    {
        LevelGenerator.coinConsumedEvent += OnCoinConsumed;
        //playerMovement.Enable();
        swipeInputActions.Enable();
    }
    private void OnDisable()
    {
        LevelGenerator.coinConsumedEvent -= OnCoinConsumed;
        //playerMovement.Disable();
        swipeInputActions.Disable();
    }

    public void ActivateShield()
    {
        Shield.SetActive(true);
        canMove = true;
        isInvincible = true;
        isSwipeAllowed = true;
        playerSpeed -= 5f;
        StartCoroutine(ShieldTimeOut());
        thisBall.SetActive(true);

    }
    IEnumerator ShieldTimeOut()
    {
        yield return new WaitForSeconds(10);
        Shield.SetActive(false);
        isInvincible = false;
    }
    public void ExplosionBoom()
    {
        SoundManager.Instance.ExplosionSound();
        explosion.Play();
        thisBall.SetActive(false);
    }
    public void GameOver()
    {
        UIManager.Instance.gameOverScore.text = score.ToString("f2");
        UIManager.Instance.gameOverCoin.text = thisRunCoinValue.ToString();
        GameLoadState.coinAmt += thisRunCoinValue;

        
        if (score >= GameLoadState.highScore)
        {
            GameLoadState.highScore = score;
            Social.ReportScore((long)score*100, leaderboard_top_score_leaderboard, LeaderBoardUpdate);
        }
        SaveAndLoadData.SaveData();
    }

    private void LeaderBoardUpdate(bool isSuccess)
    {
        if (isSuccess)
        {
            Debug.Log("Leaderboard Updated");
        }
        else
        {
            Debug.Log("Leaderboard Update Failed");
        }
    }

    private void OnCoinConsumed()
    {
        thisRunCoinValue++;
        UIManager.Instance.thisCoinAmount.text = thisRunCoinValue.ToString();
        // reflect change in ui
    }


    private void CancelInput(InputAction.CallbackContext ctx)
    {
        if (!isSwipeAllowed) return;
        isSwipePerformed = false;
        canSwipe = false;
        CancelledEvent?.Invoke();
    }

    private void CheckSwipeInput(InputAction.CallbackContext ctx)
    {
        if (!isSwipeAllowed) return;

        if (onEndEvent != null) onEndEvent(swipeInputActions.FindAction("PrimaryPosition").ReadValue<Vector2>());
        //if (onEndEvent != null) onEndEvent(playerMovement.Touch.PrimaryPosition.ReadValue<Vector2>());
    }
    private void StartTouchPrimary(InputAction.CallbackContext ctx)
    {
        if (!isSwipeAllowed) return;
        isSwipePerformed = false;
        canSwipe = true;
        if (onStartEvent != null) onStartEvent(swipeInputActions.FindAction("PrimaryPosition").ReadValue<Vector2>());

        //if (onStartEvent != null) onStartEvent(playerMovement.Touch.PrimaryPosition.ReadValue<Vector2>());
    }

    private void FixedUpdate()
    {
        MovePlayer();
        Gravity();
    }
    void Gravity()
    {
        if (rb.velocity.y < 0)
        {
            gravityScale = 9.8f;
        }
        else
        {
            gravityScale = 5f;
        }
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }
    private void MovePlayer()
    {
        if (canMove)
        {
            movePos += new Vector3(0, 0, playerSpeed) * Time.deltaTime;
            rb.position = new Vector3(rb.position.x, rb.position.y, movePos.z);
            playerObject.localRotation = playerObject.localRotation * Quaternion.Euler(playerSpeed / rotationfactor, 0, 0);
            playerSpeed += 0.2f * Time.deltaTime;
            playerSpeed = Mathf.Clamp(playerSpeed, 0f, 100f);

            score += 0.5f * Time.deltaTime;

            if (levelType == type.normal)
            {
                if (score > 50 * count)
                {
                    count++;
                    SwapLevels();
                }
            }
            lastTrackScore += 0.5f * Time.deltaTime;
            lastCoinPlaformValue += 0.5f * Time.deltaTime;
            UIManager.Instance.score.text = score.ToString("f2");
        }
    }
    void SwapLevels()
    {
        if (isLeaningLeft)
        {
            StartCoroutine(SetAllMat(0.0007f));
        }
        else
        {
            StartCoroutine(SetAllMat(-0.0007f));
        }
        isLeaningLeft = !isLeaningLeft;
    }
    IEnumerator SetAllMat(float val)
    {
        float currentValue = worldMat.GetFloat("_sidewaysStrength");

        if (currentValue < 0)
        {
            while (currentValue < val)
            {
                yield return new WaitForSeconds(0.05f);
                currentValue += 0.0001f;
                Debug.Log(currentValue);
                foreach (var item in otherMat)
                {
                    item.SetFloat("_Fwd", currentValue);
                }
                if (worldMat != null)
                {
                    worldMat.SetFloat("_sidewaysStrength", currentValue);
                }
            }
        }
        else
        {
            while (currentValue > val)
            {
                yield return new WaitForSeconds(0.05f);
                currentValue -= 0.0001f;
                Debug.Log(currentValue);
                foreach (var item in otherMat)
                {
                    item.SetFloat("_Fwd", currentValue);
                }
                if (worldMat != null)
                {
                    worldMat.SetFloat("_sidewaysStrength", currentValue);
                }
            }
        }


    }
    public bool isGrounded()
    {
        Vector3 off = new Vector3(rb.position.x, rb.position.y - groundCheckOffset, rb.position.z);
        return Physics.CheckSphere(off, 0.3f, layerMask);
    }
    public void slideDownForce()
    {
        if (!isGrounded())
        {
            rb.AddForce(Vector3.down * downPower, ForceMode.Impulse);
        }
    }
    public void Jump()
    {
        if (isGrounded())
        {
            SoundManager.Instance.jumpSound();
            float jumpPower = Mathf.Sqrt(jumpHeight * (Physics.gravity.y * 1f) * -2f) * rb.mass;
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }
    }
}

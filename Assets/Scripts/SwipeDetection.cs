using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
    [SerializeField]
    private float minDistance;
    [SerializeField]
    private float directionThreshold, lerpDuration;

    public Vector2 startPosition;
    public Vector2 endPosition;

    private int platformCount;
    [SerializeField] float pos1, pos2, pos3;

    private Rigidbody rb;

    bool startPosSet;
    private void Start()
    {
        platformCount = 1;
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        PlayerController.Instance.onStartEvent += SwipeStart;
        PlayerController.Instance.onEndEvent += SwipeEnd;
        //PlayerController.Instance.CancelledEvent += SwipeEventEnd;
    }
    void SwipeEventEnd()
    {
        startPosSet = false;
    }
    private void SwipeStart(Vector2 pos)
    {
        Debug.Log( pos);

        startPosSet = true;
        startPosition = pos;
    }

    private void SwipeEnd(Vector2 pos)
    {
        Debug.Log(pos);
        if(startPosSet && !PlayerController.Instance.isSwipePerformed)
        {
            endPosition = pos;
            SwipeMovement();
        }
    }
    void SwipeMovement()
    {
        if(PlayerController.Instance.canSwipe && !PlayerController.Instance.isSwipePerformed)
        {
            if (Vector3.Distance(startPosition, endPosition) >= minDistance)
            {
                PlayerController.Instance.isSwipePerformed = true;
                Vector3 direction = endPosition - startPosition;
                Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
                SwipeDirection(direction2D);
            }
        }
    }
    private void OnDisable()
    {
        PlayerController.Instance.onStartEvent -= SwipeStart;
        PlayerController.Instance.onEndEvent -= SwipeEnd;
    }

    private void SwipeDirection(Vector2 direction)
    {
        if (Vector2.Dot(Vector2.up, direction) > directionThreshold)
        {
            PlayerController.Instance.Jump();
        }
        else if (Vector2.Dot(Vector2.down, direction) > directionThreshold)
        {
            PlayerController.Instance.slideDownForce();
            SoundManager.Instance.swipeDownSound();

        }
        else if (Vector2.Dot(Vector2.left, direction) > directionThreshold)
        {
            if (platformCount == 0) return;
            SoundManager.Instance.swipeSideWaysSound();
            platformCount--;
            rb.position = Vector3.Lerp(rb.position, MoveCharacter(platformCount)/*+new Vector3(0,0,10)*/, lerpDuration);
        }
        else if (Vector2.Dot(Vector2.right, direction) > directionThreshold)
        {
            if (platformCount == 2) return;
            SoundManager.Instance.swipeSideWaysSound();
            platformCount++;
            rb.position = Vector3.Lerp(rb.position, MoveCharacter(platformCount)/*+new Vector3(0,0,10)*/, lerpDuration);
        }
    }
    private Vector3 MoveCharacter(int count)
    {
        PlayerController.Instance.movePos += new Vector3(0, 0, 3);
        switch (count)
        {
            case 0:
                return new Vector3(pos1, rb.position.y, rb.position.z);
            case 1:
                return new Vector3(pos2, rb.position.y, rb.position.z);
            case 2:
                return new Vector3(pos3, rb.position.y, rb.position.z);
            default:
                return new Vector3(pos2, rb.position.y, rb.position.z);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField]
    private Camera cam; 
    [SerializeField]
    private SnakeHead snakeHead;
    [SerializeField]
    private List<Vector3> PositionBuffer = new List<Vector3>();
    [SerializeField, Range(1, 10)]
    private float forwardSpeed = 5f, fewerMultiplier = 3f;
    [SerializeField, Range(1, 50)]
    private float sideSpeed = 7f, rotateSpeed = 2f;
    [SerializeField]
    private float maxNodeDistanceBetween = 0.75f;

    private Coroutine fewerVelocityCoroutine;
    private Vector3 velocity;
    private readonly float rotationThreshold = 1f, movementBoundsX = 1.1f, movementThreshold = 0.05f;
    private readonly int savedFramesPerTailNode = 7;
    private float currFewerMultiplier = 1f;

    private void Awake()
    {
        cam ??= Camera.main;
    }
    private void Start()
    {
        ResetXVelocity();
        for (int i = 0; i < savedFramesPerTailNode * (snakeHead.TailNodes.Count + 1); i++)
        {
            PositionBuffer.Add(transform.position);
        }
    }
    private void Update()
    {
        ChangePositionAndRotation(transform, velocity);

        for (int i = 1; i < snakeHead.TailNodes.Count; i++)
        {
            if (PositionBuffer.Count >= i * savedFramesPerTailNode)
            {
                Vector3 nextPos = PositionBuffer[PositionBuffer.Count - i * savedFramesPerTailNode];
                nextPos.y = 0.25f;
                if(snakeHead.TailNodes[i - 1].transform.position.z - nextPos.z >= maxNodeDistanceBetween)
                {
                    nextPos.z = transform.position.z - i * maxNodeDistanceBetween;
                }
                snakeHead.TailNodes[i].transform.LookAt(nextPos);
                snakeHead.TailNodes[i].transform.position = nextPos;
            }
        }
    }
    private void LateUpdate()
    {
        PositionBuffer.Add(transform.position);
        if (PositionBuffer.Count > savedFramesPerTailNode * snakeHead.TailNodes.Count)
        {
            PositionBuffer.RemoveAt(0);
        }
    }
    private void ChangePositionAndRotation(Transform target, Vector3 velocity)
    {
        target.transform.Translate(velocity * Time.deltaTime, Space.World);

        if (Mathf.Abs(velocity.x) < rotationThreshold)
        {
            target.transform.rotation =
                Quaternion.RotateTowards(target.transform.rotation, Quaternion.LookRotation(velocity * Time.deltaTime), rotateSpeed);
        }
        else
        {
            target.transform.rotation = Quaternion.LookRotation(velocity * Time.deltaTime);
        }
    }
    private void UpdateVelocity(Vector2 touchPos)
    {

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(touchPos);

        float distanceDiff = 0f;

        if (Physics.Raycast(ray, out hit))
        {
            distanceDiff = Mathf.Clamp(hit.point.x, -movementBoundsX, movementBoundsX);
            distanceDiff -= transform.position.x;
        }

        if (Mathf.Abs(distanceDiff) < movementThreshold) distanceDiff = 0;

        velocity = new Vector3(distanceDiff * sideSpeed, 0, forwardSpeed * currFewerMultiplier);
    }


    private void ResetXVelocity()
    {
        velocity = new Vector3(0, 0, forwardSpeed * currFewerMultiplier);
    }

    private void ChangeFewerMode(bool isFewer)
    {
        currFewerMultiplier = isFewer ? fewerMultiplier : 1f;
        if (isFewer)
        {
            GameInputManager.Current.OnTouchHappend -= UpdateVelocity;
            GameInputManager.Current.OnTouchEnded -= ResetXVelocity;
            ResetXVelocity();
            fewerVelocityCoroutine = StartCoroutine(fewerVelocity());
        } 
        else
        {
            GameInputManager.Current.OnTouchHappend += UpdateVelocity;
            GameInputManager.Current.OnTouchEnded += ResetXVelocity;
            StopCoroutine(fewerVelocityCoroutine);
            ResetXVelocity();
        }
    }
    private IEnumerator fewerVelocity()
    {
        while (true)
        {
            if (Mathf.Abs(transform.position.x) > movementThreshold)
            {
                velocity = velocity = new Vector3(-transform.position.x * sideSpeed, 0, forwardSpeed * currFewerMultiplier);
            }
            else
            {
                ResetXVelocity();
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void OnEnable()
    {
        GameInputManager.Current.OnTouchHappend += UpdateVelocity;
        GameInputManager.Current.OnTouchEnded += ResetXVelocity;
        GameManager.Current.OnFewerStateChanged += ChangeFewerMode;
    }
    private void OnDisable()
    {
        GameInputManager.Current.OnTouchHappend -= UpdateVelocity;
        GameInputManager.Current.OnTouchEnded -= ResetXVelocity;
        if (GameManager.Current != null)
        {
            GameManager.Current.OnFewerStateChanged -= ChangeFewerMode;
        }
    }
}

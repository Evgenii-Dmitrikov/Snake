using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    public event Action OnEatFood;
    public event Action OnEatGems;
    public event Action OnEatBad;

    
    public SegmentColor currColor;

    public List<Transform> TailNodes;

    [SerializeField]
    private GameObject tailPrefab;

    [Header("Gameplay values")]
    [SerializeField, Range(4, 10)]
    public int MaxTailSegments = 6;
    [SerializeField]
    public float EatRadius = 0.6f;
    [SerializeField, Range(0,360)]
    public float EatAngle = 90f;
    [SerializeField]
    private float eatAnimDuration = 0.2f, scaleAnimTime = 0.5f;
    [SerializeField]
    private LayerMask normalEadible, gemsMask, obstacleMask;

    private Coroutine eatingProcessing;

    private LayerMask curEadibleMask, curBlockerMask;

    private Vector3 scale;
    private bool isFewerOn;

    private void Awake()
    {
        curEadibleMask = normalEadible | gemsMask;
        curBlockerMask = obstacleMask;
        scale = transform.localScale;
    }
    private void Start()
    {
        StartCoroutine(FindTargetsWithDelay(0.05f));
    }
    private IEnumerator StartEating()
    {
        StartCoroutine(EatingLerp());
        yield return new WaitForSeconds(scaleAnimTime);
        eatingProcessing = null;
        yield break;
    }
    private IEnumerator EatingLerp()
    {
        for (int i = 0; i < TailNodes.Count; i++)
        {
            float t = 0;
            while (t < scaleAnimTime * 0.5f)
            {
                TailNodes[i].localScale = Vector3.Lerp(scale, scale * 1.3f, t / (scaleAnimTime * 0.5f));
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            var _scale = TailNodes[i].localScale;

            while (t < scaleAnimTime)
            {
                TailNodes[i].localScale = Vector3.Lerp(_scale, scale, t / scaleAnimTime);
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            t = 0;
        }
    }
    private IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindEdibleTargets();
        }
    }
    private IEnumerator LerpColors(Color colorTo)
    {
        for (int i = 0; i < TailNodes.Count; i++)
        { 
            var colorChanger = TailNodes[i].GetComponent<ColorChanger>();
            colorChanger.SetNewColor(colorTo);
            yield return new WaitForSeconds(colorChanger.duration);
        }
    }

    private void FindEdibleTargets()
    {
        Collider[] eadibleTargetsInRadius = Physics.OverlapSphere(transform.position, EatRadius, curEadibleMask);

        for (int i = 0; i < eadibleTargetsInRadius.Length; i++)
        {
            Transform target = eadibleTargetsInRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < EatAngle / 2)
            {
                if (IsInLayerMask(target.gameObject, obstacleMask) && isFewerOn)
                {
                    target.GetComponent<EatingAnim>()?.Eaten(transform, eatAnimDuration);
                    if (eatingProcessing == null)
                    {
                        eatingProcessing = StartCoroutine(StartEating());
                    }
                }
                else if (target.GetComponent<Colorable>()?.curColor == currColor || (isFewerOn && target.GetComponent<Colorable>() != null))
                {
                    OnEatFood?.Invoke();
                    target.GetComponent<EatingAnim>()?.Eaten(transform, eatAnimDuration);
                    if (eatingProcessing == null)
                    {
                        eatingProcessing = StartCoroutine(StartEating());
                    }
                }
                else if (IsInLayerMask(target.gameObject, gemsMask))
                {
                    OnEatGems?.Invoke();
                    target.GetComponent<EatingAnim>()?.Eaten(transform, isFewerOn ? eatAnimDuration : 0f);
                    if (eatingProcessing == null)
                    {
                        eatingProcessing = StartCoroutine(StartEating());
                    }
                }
                else
                {
                    OnEatBad?.Invoke();
                }
                eadibleTargetsInRadius[i].enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if ((curBlockerMask & 1 << collider.gameObject.layer) == 1 << collider.gameObject.layer)
        {
            OnEatBad?.Invoke();
        }
    }

    public void SetNewColor(SegmentColor colorTo)
    {
        //нет проверки на уже смену цвета, но так часто не мен€етс€
        currColor = colorTo;
        StartCoroutine(LerpColors(GameManager.colors[(int)colorTo]));
    }
    public void AddTailNode()
    {
        if (TailNodes.Count < MaxTailSegments)
        {
            TailNodes.Add(Instantiate
                (
                tailPrefab,
                TailNodes[TailNodes.Count - 1].position,
                Quaternion.identity,
                TailNodes[TailNodes.Count - 1].transform.parent
                ).transform);
            TailNodes[TailNodes.Count - 1].GetComponent<ColorChanger>().SetNewColorIntant(GameManager.colors[(int)currColor]);
        }
    }

    private void fewerToggle(bool fewerIsOn)
    {
        isFewerOn = fewerIsOn;
        EatRadius *= fewerIsOn ? 5 : 0.2f;
        if (fewerIsOn)
        {
            curEadibleMask = normalEadible | obstacleMask | gemsMask;
            curBlockerMask = default;
        }
        else
        {
            curEadibleMask = normalEadible | gemsMask;
            curBlockerMask = obstacleMask;
        }

        // curEadibleMask = fewerIsOn ? normalEadible | obstacleMask : normalEadible;
    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        return ((layerMask.value & (1 << obj.layer)) > 0);
    }
    private void OnEnable()
    {
        GameManager.Current.OnFewerStateChanged += fewerToggle;
    }
    private void OnDisable()
    {
        if (GameManager.Current != null)
        {
            GameManager.Current.OnFewerStateChanged -= fewerToggle;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowDummy : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private void LateUpdate()
    {
        transform.position = target.transform.position;
    }
}

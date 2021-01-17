using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCanvas : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector2 offset;


    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position + (Vector3)offset;
    }
}

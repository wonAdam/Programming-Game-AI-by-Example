using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float followSpeed;
    [SerializeField] float followRange;
    void Start()
    {
        
    }

    void LateUpdate()
    {
        if(!IsTargetInBox())
        {
            LerpPositionToTarget();
        }
    }

    public bool IsTargetInBox() => Vector2.Distance(transform.position, target.position) <= followRange;
    
    private void LerpPositionToTarget()
    {
        Vector3 ToTarget = target.position - transform.position;
        ToTarget.z = -10f;
        transform.position = Vector3.Lerp(transform.position, transform.position + ToTarget.normalized * followRange, followSpeed * Time.deltaTime);
        
    }
}

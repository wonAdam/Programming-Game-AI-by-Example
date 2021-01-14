using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] float followSpeed;
    [SerializeField] float followRange;
    [SerializeField] float moveSpeedByKey;


    private void Update()
    {
        if(!target)
        {
            float v = Input.GetAxis("Vertical Arrow");
            float h = Input.GetAxis("Horizontal Arrow");

            transform.Translate(new Vector3(h * moveSpeedByKey * Time.deltaTime, v * moveSpeedByKey * Time.deltaTime, 0f));
        }
    }


    void LateUpdate()
    {
        if(target && !IsTargetInBox())
        {
            LerpPositionToTarget();
        }

        
    }

    public bool IsTargetInBox() => Vector2.Distance(transform.position, target.position) <= followRange;
    
    private void LerpPositionToTarget()
    {
        Vector3 ToTarget = target.position - transform.position;
        Vector2 nextPos = Vector3.Lerp(transform.position, transform.position + ToTarget.normalized * followRange, followSpeed * Time.deltaTime);
        transform.position = new Vector3(nextPos.x, nextPos.y, -10f);
    }
}

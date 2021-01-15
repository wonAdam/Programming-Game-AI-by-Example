using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterposeAgent : MovingEntity
{

    [SerializeField] Transform target;
    SteeringBehaviors steeringBehaviors;
    Rigidbody2D rigidbody2D;
    // Start is called before the first frame update
    void Start()
    {
        steeringBehaviors = GetComponent<SteeringBehaviors>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        // SteeringBehaviors state에 따라 velocity를 갱신합니다.
        ProcessSteeringBehaviors();
        
        if(Vector2.Distance(transform.position, targetTr.position) < 0.1f)
        {
            if(FindObjectOfType<InterposeMgr>().agentAArrived)
                FindObjectOfType<InterposeMgr>().agentBArrived = true;
            else
                FindObjectOfType<InterposeMgr>().agentAArrived = true;
        }
    }

    private void ProcessSteeringBehaviors()
    {
        Vector2 steeringForce = steeringBehaviors.Calculate();
        Vector2 acceleration = steeringForce / GetComponent<Rigidbody2D>().mass;
        rigidbody2D.velocity += acceleration * Time.deltaTime;
        if (acceleration.magnitude < Mathf.Epsilon)
            rigidbody2D.velocity -= rigidbody2D.velocity.normalized * Time.deltaTime; // 더해진게 없을 시 감속

        rigidbody2D.velocity = Vector2.ClampMagnitude(rigidbody2D.velocity, maxSpeed);
    }
}

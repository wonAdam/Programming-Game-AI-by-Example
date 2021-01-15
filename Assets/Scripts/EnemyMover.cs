using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MovingEntity
{
    public SteeringBehaviors m_pSteering;

    new Rigidbody2D rigidbody2D;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        m_pSteering = GetComponent<SteeringBehaviors>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // SteeringBehaviors state에 따라 velocity를 갱신합니다.
        ProcessSteeringBehaviors();
    }

    private void ProcessSteeringBehaviors()
    {
        Vector2 steeringForce = m_pSteering.Calculate();
        Vector2 acceleration = steeringForce / GetComponent<Rigidbody2D>().mass;
        rigidbody2D.velocity += acceleration * Time.deltaTime;
        if(acceleration.magnitude < Mathf.Epsilon)
            rigidbody2D.velocity -= rigidbody2D.velocity.normalized * Time.deltaTime; // 더해진게 없을 시 감속

        rigidbody2D.velocity = Vector2.ClampMagnitude(rigidbody2D.velocity, maxSpeed);
    }

    private void OnDrawGizmos()
    {
        if(!rigidbody2D)
            rigidbody2D = GetComponent<Rigidbody2D>();

        Debug.DrawLine(transform.position, (Vector2)transform.position + rigidbody2D.velocity, Color.blue);
    }
}

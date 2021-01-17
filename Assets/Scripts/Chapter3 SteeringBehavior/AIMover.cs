using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D), typeof(SteeringBehaviors))]
public class AIMover : MovingEntity
{
    public SteeringBehaviors steeringBehavior;
    Rigidbody2D rigidbody2D;

    [Header ("Only for Player in Interpose Mode")]
    [SerializeField] public MovingEntity agentA;
    [SerializeField] public MovingEntity agentB;

    private void Start()
    {
        steeringBehavior = GetComponent<SteeringBehaviors>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        ProcessSteeringBehaviors();

        // Interpose Mode Specific
        if (agentA && agentB &&
            Vector2.Distance(transform.position, agentA.transform.position) < 0.1f &&
            Vector2.Distance(transform.position, agentB.transform.position) < 0.1f)
        {
            FindObjectOfType<InterposeMgr>().playerArrived = true;
        }
    }

    private void ProcessSteeringBehaviors()
    {
        Vector2 steeringForce = steeringBehavior.Calculate(this);
        Vector2 acceleration = steeringForce / GetComponent<Rigidbody2D>().mass;

        if (!float.IsNaN(acceleration.x))
            rigidbody2D.velocity += acceleration * Time.deltaTime;

        if (acceleration.magnitude < Mathf.Epsilon)
            rigidbody2D.velocity -= CalculateDragForce() * Time.deltaTime; // 더해진게 없을 시 감속

        rigidbody2D.velocity = Vector2.ClampMagnitude(rigidbody2D.velocity, maxSpeed);
    }

    private Vector2 CalculateDragForce() => rigidbody2D.velocity.normalized * rigidbody2D.velocity.magnitude / 2f;

    private void OnDrawGizmos()
    {
        if(!rigidbody2D)
            rigidbody2D = GetComponent<Rigidbody2D>();

        Debug.DrawLine(transform.position, (Vector2)transform.position + rigidbody2D.velocity, Color.blue);
    }
}

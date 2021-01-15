﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterposePlayer : MovingEntity
{
    [SerializeField] public InterposeAgent agentA;
    [SerializeField] public InterposeAgent agentB;
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


        if (Vector2.Distance(transform.position, agentA.transform.position) < 0.1f &&
            Vector2.Distance(transform.position, agentB.transform.position) < 0.1f)
        {
            FindObjectOfType<InterposeMgr>().playerArrived = true;
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
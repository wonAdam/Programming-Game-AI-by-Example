﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerMover : MovingEntity
{
    Rigidbody2D rigidbody2D;
    AnimationHandler animHandler;
    float v = 0f, h = 0f;
    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animHandler = GetComponent<AnimationHandler>();
    }
    private void Update()
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");

        // TODO - should seperate from mover
        if(animHandler && Input.GetKeyDown(KeyCode.Space))
        {
            animHandler.AttackAnimation();
        }
    }

    void LateUpdate()
    {
        Vector2 velocity = (new Vector2(h, v) * Time.deltaTime).normalized * maxSpeed;
        rigidbody2D.velocity = velocity;
    }
    private void OnDrawGizmos()
    {
        if (!rigidbody2D) rigidbody2D = GetComponent<Rigidbody2D>();

        Debug.DrawLine(transform.position, (Vector2)transform.position + rigidbody2D.velocity, Color.red);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MovingEntity
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

        if(Input.GetKeyDown(KeyCode.Space))
        {
            animHandler.AttackAnimation();
        }
    }

    void FixedUpdate()
    {
        Vector2 velocity = new Vector2(h, v).normalized * maxSpeed;
        rigidbody2D.velocity = velocity;
    }
    private void OnDrawGizmos()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        Debug.DrawLine(transform.position, (Vector2)transform.position + rigidbody2D.velocity, Color.red);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MovingEntity
{
    Rigidbody2D rigidbody2D;
    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Vector2 velocity = new Vector2(h, v).normalized * maxSpeed;
        rigidbody2D.velocity = velocity;
    }
    private void OnDrawGizmos()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        Debug.DrawLine(transform.position, (Vector2)transform.position + rigidbody2D.velocity, Color.red);
    }
}

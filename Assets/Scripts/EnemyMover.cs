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
        if (!float.IsNaN(acceleration.x))
            rigidbody2D.velocity += acceleration * Time.deltaTime;
        if(acceleration.magnitude < Mathf.Epsilon)
            rigidbody2D.velocity -= (rigidbody2D.velocity.normalized * rigidbody2D.velocity.magnitude / 2f) * Time.deltaTime; // 더해진게 없을 시 감속

        rigidbody2D.velocity = Vector2.ClampMagnitude(rigidbody2D.velocity, maxSpeed);
    }

    [Header ("Only For Hide Mode")]
    [SerializeField] float rightAngle;
    [SerializeField] float leftAngle;
    [SerializeField] public float sightLength;

    private void OnDrawGizmos()
    {
        if(!rigidbody2D)
            rigidbody2D = GetComponent<Rigidbody2D>();
        if (!m_pSteering)
            m_pSteering = GetComponent<SteeringBehaviors>();

        Debug.DrawLine(transform.position, (Vector2)transform.position + rigidbody2D.velocity, Color.blue);


        if(FindObjectOfType<PlayerHide>()?.GetComponent<SteeringBehaviors>().state == SteeringBehaviors.State.Hide)
        {
            Vector2 rightAngleSight = Quaternion.Euler(0f, 0f, rightAngle) * transform.right * sightLength * 2f;
            Debug.DrawLine(transform.position, transform.position + (Vector3)rightAngleSight, Color.red);
            Vector2 leftAngleSight = Quaternion.Euler(0f, 0f, leftAngle) * transform.right * sightLength * 2f;
            Debug.DrawLine(transform.position, transform.position + (Vector3)leftAngleSight, Color.red);
        }
    }
}

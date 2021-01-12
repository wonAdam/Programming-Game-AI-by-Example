using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviors : MonoBehaviour
{
    public enum State
    {
        Seek, Flee, Arrive, Pursuit 
    }
    public State state = State.Seek;

    public enum Deceleration
    {
        fast = 1, normal = 2, slow = 3
    }
    public Deceleration deceleration = Deceleration.slow;

    [SerializeField] float panicDistance = 7f;

    public MovingEntity movingEntity;
    private void Start()
    {
        movingEntity = GetComponent<MovingEntity>();
    }

    /// <summary>
    /// State에 따른 적절한 다음 MovingEntity에 줘야할 Force를 리턴합니다.
    /// </summary>
    /// <returns></returns>
    public Vector2 Calculate()
    {
        switch (state)
        {
            case State.Seek:
                return Seek(GetComponent<Enemy>().targetTr.position);
            case State.Flee:
                return Flee(GetComponent<Enemy>().targetTr.position);
            case State.Arrive:
                return Arrive(GetComponent<Enemy>().targetTr.position, deceleration);
            case State.Pursuit:
                return Pursuit(GetComponent<Enemy>().targetTr.GetComponent<Rigidbody2D>());
            default:
                return Vector2.zero;

        }

    }

    [Header("Vector2.Dot(ToEvader, selfHeading):")]
    [SerializeField] float Vector2Dot;
    [Header("Vector2.Dot(evaderHeading, selfHeading):")]
    [SerializeField] float relativeHeading;
    [Header("Mathf.Acos(relativeHeading) / Mathf.Deg2Rad:")]
    [SerializeField] float MathfAcos;
    private Vector2 Seek(Vector2 TargetPos)
    {
        Vector2 desiredVelocity = (TargetPos - (Vector2)transform.position).normalized * movingEntity.maxSpeed;
        return desiredVelocity - GetComponent<Rigidbody2D>().velocity;
    }

    private Vector2 Flee(Vector2 TargetPos)
    {
        if(Vector2.Distance(TargetPos, transform.position) > panicDistance)
        {
            return Vector2.zero;
        }
        Vector2 desiredVelocity = ((Vector2)transform.position - TargetPos).normalized * movingEntity.maxSpeed;
        return desiredVelocity - GetComponent<Rigidbody2D>().velocity;
    }

    private Vector2 Arrive(Vector2 TargetPos, Deceleration deceleration)
    {
        Vector2 ToTarget = TargetPos - (Vector2)transform.position;

        float distance = ToTarget.magnitude;

        if(distance > Mathf.Epsilon)
        {
            const float decelerationTweaker = 0.3f;

            float speed = distance / ((float)deceleration * decelerationTweaker);

            speed = Mathf.Min(speed, GetComponent<MovingEntity>().maxSpeed);


            Vector2 desiredVelocity = ToTarget * speed / distance;

            return desiredVelocity - GetComponent<Rigidbody2D>().velocity;

        }

        return Vector2.zero;
    }

    private Vector2 Pursuit(Rigidbody2D evader)
    {
        Vector2 ToEvader = evader.transform.position - transform.position;
        Vector2 evaderHeading = evader.velocity.normalized;
        Vector2 selfHeading = this.GetComponent<Rigidbody2D>().velocity.normalized;
        float relativeHeading = Vector2.Dot(evaderHeading, selfHeading);


        //// debug
        this.Vector2Dot = Vector2.Dot(ToEvader, selfHeading);
        this.relativeHeading = relativeHeading;
        this.MathfAcos = Mathf.Acos(relativeHeading);
        if (Vector2.Dot(ToEvader, selfHeading) > 0 && // 대면하고
            relativeHeading < Mathf.Cos(160f)) // 서로의 방향이 160도를 넘을 시
        {
            // 그냥 evader의 위치로 Seek
            Debug.Log("evader");
            return Seek(evader.transform.position);
        }

        float lookAheadTime = ToEvader.magnitude / (GetComponent<MovingEntity>().maxSpeed + evader.velocity.magnitude);
        Debug.Log("lookAheadTime");

        return Seek((Vector2)evader.transform.position + evader.velocity * lookAheadTime);
    }
}

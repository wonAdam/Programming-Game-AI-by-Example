using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SteeringBehaviors : MonoBehaviour
{
    public enum State
    {
        Seek, Flee, Arrive, Pursuit, Evade, Wander, ObstacleAvoidance, WallAvoidance
    }
    public State state = State.Seek;

    public enum Deceleration
    {
        fast = 1, normal = 2, slow = 3
    }
    public Deceleration deceleration = Deceleration.slow;

    [SerializeField] float panicDistance = 7f;

    public MovingEntity movingEntity;
    public Vector2 wanderTarget;
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
            case State.Evade:
                return Evade(GetComponent<Enemy>().targetTr.GetComponent<Rigidbody2D>());
            case State.Wander:
                return Wander();
            case State.ObstacleAvoidance:
                return ObstacleAvoidance();
            default:
                return Vector2.zero;

        }

    }

    ////debug
    //[Header("Vector2.Dot(ToEvader, selfHeading):")]
    //[SerializeField] float Vector2Dot;
    //[Header("Vector2.Dot(evaderHeading, selfHeading):")]
    //[SerializeField] float relativeHeading;
    //[Header("Mathf.Acos(relativeHeading) / Mathf.Deg2Rad:")]
    //[SerializeField] float MathfAcos;
    private Vector2 Seek(Vector2 TargetPos)
    {
        Vector2 desiredVelocity = (TargetPos - (Vector2)transform.position).normalized * movingEntity.maxSpeed;
        return desiredVelocity - GetComponent<Rigidbody2D>().velocity + ObstacleAvoidance();
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


        ////// debug
        //this.Vector2Dot = Vector2.Dot(ToEvader, selfHeading);
        //this.relativeHeading = relativeHeading;
        //this.MathfAcos = Mathf.Acos(relativeHeading);
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

    private Vector2 Evade(Rigidbody2D pursuer)
    {
        Vector2 ToPursuer = pursuer.transform.position - transform.position;

        float lookAheadTime = ToPursuer.magnitude /
            (movingEntity.maxSpeed + pursuer.GetComponent<MovingEntity>().maxSpeed);

        return Flee((Vector2)pursuer.transform.position + pursuer.velocity * lookAheadTime);
    }

    [SerializeField] float wanderRadius;
    [SerializeField] float wanderDistance;
    [SerializeField] float wanderJitter;
    
    private Vector2 Wander()
    {
        Vector2 targetLocal;
        Vector2 targetWorld;

        wanderTarget += new Vector2(UnityEngine.Random.Range(-1f, 1f) * wanderJitter,
            UnityEngine.Random.Range(-1f, 1f) * wanderJitter);

        wanderTarget = wanderTarget.normalized;

        wanderTarget *= wanderRadius;

        targetLocal = wanderTarget + new Vector2(wanderDistance, 0f);

        targetWorld = (Vector2)transform.position + targetLocal;

        return targetWorld - (Vector2)transform.position;

    }

    [SerializeField] float minDetectionBoxLength;
    private Vector2 ObstacleAvoidance()
    {
        // Cast DetectionBox
        Vector2 direction = GetComponent<Rigidbody2D>().velocity;
        List<RaycastHit2D> results = new List<RaycastHit2D>();
        float boxLength = CalcBoxLength();
        GetComponent<Collider2D>().Cast(direction, GetContactFilter2D(), results, boxLength);

        // Mark All Obstacles within the DetectionBox
        // done by results List

        if(results.Count == 0)
            return Vector2.zero;

        // Find a Nearest Obstacle
        Collider2D nearestIntersectingObstacle = GetNearestIntersectingObstacle(results);

        // Calculate SteeringForce
        Vector2 steeringForce = CalcSteeringForce(boxLength, nearestIntersectingObstacle);

        return steeringForce;
    }

    //debug
    [SerializeField] Vector2 localPosOfObstacle;
    [SerializeField] float obstacleXRadius;
    [SerializeField] float obstacleYRadius;
    [SerializeField] Vector2 boundsSize;
    private Vector2 CalcSteeringForce(float boxLength, Collider2D nearestIntersectingObstacle)
    {
        float steeringForceX, steeringForceY; 
        Vector2 currDirection = GetComponent<Rigidbody2D>().velocity.normalized;
        Vector2 localPosOfObstacle = nearestIntersectingObstacle.transform.position - transform.position;


        // y
        float multiplier = 1.0f + (boxLength - Vector2.Dot(localPosOfObstacle, currDirection)) / boxLength;
        float obstacleYRadius = nearestIntersectingObstacle.bounds.size.y;
        steeringForceY =
            localPosOfObstacle.y > 0 ?
            (localPosOfObstacle.y - obstacleYRadius) * multiplier : (obstacleYRadius - localPosOfObstacle.y) * multiplier
            ;

        // x
        const float brakingWeight = 0.5f;
        float obstacleXRadius = nearestIntersectingObstacle.bounds.size.x;
        steeringForceX = (localPosOfObstacle.x - obstacleXRadius) * brakingWeight;

        //debug
        this.boundsSize = nearestIntersectingObstacle.bounds.size;
        this.localPosOfObstacle = localPosOfObstacle;
        this.obstacleXRadius = obstacleXRadius;
        this.obstacleYRadius = obstacleYRadius;

        return new Vector2(steeringForceX, steeringForceY);
    }
    private Collider2D GetNearestIntersectingObstacle(List<RaycastHit2D> results)
    {
        Collider2D nearestIntersectingObstacle = null;
        foreach (var r in results)
        {
            if (nearestIntersectingObstacle == null)
            {
                nearestIntersectingObstacle = r.collider;
                continue;
            }

            float distOfNearestObstacle = Vector2.Distance(transform.position, nearestIntersectingObstacle.transform.position);
            float distOfCurrResultObstacle = Vector2.Distance(transform.position, r.transform.position);
            if (distOfCurrResultObstacle < distOfNearestObstacle) nearestIntersectingObstacle = r.collider;
        }

        return nearestIntersectingObstacle;
    }
    private ContactFilter2D GetContactFilter2D()
    {
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.useLayerMask = true;
        contactFilter2D.layerMask.value |= LayerMask.GetMask("Obstacle");
        return contactFilter2D;
    }
    private float CalcBoxLength() => minDetectionBoxLength + (GetComponent<Rigidbody2D>().velocity.magnitude / movingEntity.maxSpeed) * minDetectionBoxLength;

    //private void OnDrawGizmos()
    //{
    //    if(state == State.ObstacleAvoidance)
    //    {


    //    }
    //}

}

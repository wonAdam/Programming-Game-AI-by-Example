using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SteeringBehaviors : MonoBehaviour
{
    public enum State
    {
        Seek, Flee, Arrive, Pursuit, Evade, Wander, ObstacleAvoidance
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
                return ObstacleAvoidance(GetComponent<Enemy>().targetTr.position);
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
        Vector2 desiredVelocity = 
            (TargetPos - (Vector2)transform.position).normalized * movingEntity.maxSpeed;
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

    [SerializeField] Collider2D lastTimeObstacle;
    [SerializeField] float minDetectionBoxLength;
    private Vector2 ObstacleAvoidance(Vector2 TargetPos)
    {
        // Cast DetectionBox
        // Mark All Obstacles within the DetectionBox
        Vector2 direction = TargetPos - (Vector2)transform.position;
        List<RaycastHit2D> results = new List<RaycastHit2D>();
        float boxLength = CalcBoxLength();
        GetComponent<Collider2D>().Cast(direction, GetContactFilter2D(), results, boxLength);

        Matrix4x4 localToWorld = Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.FromToRotation(Vector3.right, direction),
            Vector3.one
            );

        // velocity == 0 || no obstacle front
        if (results.Count == 0)
        {
            return Seek(TargetPos);
        }

        // Find a Nearest Obstacle
        Collider2D nearestIntersectingObstacle = GetNearestIntersectingObstacle(results);
        lastTimeObstacle = nearestIntersectingObstacle;

        // Calculate SteeringLocalForce
        Vector2 steeringForceLocal = CalcSteeringLocalForce(boxLength, nearestIntersectingObstacle, localToWorld);

        //debug
        this.steeringForceLocal = steeringForceLocal;

        // Project LocalForce To World
        Vector2 steeringForceWorld = localToWorld.MultiplyPoint3x4(steeringForceLocal);
            //worldToLocal.inverse.MultiplyPoint3x4(steeringForceLocal);
        this.steeringForceWorld = steeringForceWorld;

        //if(direction.x < 0) steeringForceWorld.y *= -1f;
        Vector2 desiredVelocity = (Seek(TargetPos) + steeringForceWorld).normalized * movingEntity.maxSpeed;
        return desiredVelocity - GetComponent<Rigidbody2D>().velocity;
    }

    //debug
    [SerializeField] Vector2 steeringForceLocal;
    [SerializeField] Vector2 steeringForceWorld = new Vector2(0f,0f);
    [SerializeField] Vector2 localPosOfObstacle;
    [SerializeField] float obstacleXRadius;
    [SerializeField] float obstacleYRadius;
    [SerializeField] Vector2 boundsSize;
    
    private Vector2 CalcSteeringLocalForce(float boxLength, Collider2D nearestIntersectingObstacle, Matrix4x4 localToWorld)
    {
        // project nearestIntersectingObstacle to local coordinate
        float steeringLocalForceX, steeringLocalForceY;

        Vector2 localPosOfObstacle =
            localToWorld.inverse.MultiplyPoint3x4(nearestIntersectingObstacle.transform.position - transform.position);

        // y
        float multiplier = 1.0f + (boxLength - localPosOfObstacle.x) / boxLength;
        float obstacleYRadius = nearestIntersectingObstacle.bounds.size.y;
        steeringLocalForceY = SignReverse(localPosOfObstacle.y) * GetAbsDiff(obstacleYRadius, localPosOfObstacle.y) * multiplier;

        // x
        const float brakingWeight = 0.2f;
        float obstacleXRadius = nearestIntersectingObstacle.bounds.size.x;
        steeringLocalForceX = (obstacleXRadius - localPosOfObstacle.x) * brakingWeight;

        //debug
        this.boundsSize = nearestIntersectingObstacle.bounds.size;
        this.localPosOfObstacle = localPosOfObstacle;
        this.obstacleXRadius = obstacleXRadius;
        this.obstacleYRadius = obstacleYRadius;

        return new Vector2(steeringLocalForceX, steeringLocalForceY);
    }

    private static float SignReverse(float v)
    {
        return (-1f) * Mathf.Sign(v);
    }

    private float GetAbsDiff(float v1, float v2) => Mathf.Abs(Mathf.Abs(v1) - Mathf.Abs(v2));

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

    private void OnDrawGizmos()
    {
        Debug.DrawLine(Vector2.zero, (Vector3)steeringForceWorld, Color.yellow);
        Debug.DrawLine(transform.position, transform.position + (Vector3)steeringForceLocal, Color.blue);
    }

}

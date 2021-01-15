using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SteeringBehaviors : MonoBehaviour
{
    public enum State
    {
        Seek, Flee, Arrive, Pursuit, Evade, Wander, ObstacleAvoidance, WallAvoidance
    }

    public enum Deceleration
    {
        fast = 1, normal = 2, slow = 3
    }

    private MovingEntity movingEntity;
    private Vector2 wanderTarget;
    private Collider2D lastTimeObstacle;
    public State state = State.Seek;

    [SerializeField] Deceleration deceleration = Deceleration.slow;
    [SerializeField] float panicDistance = 7f;
    [SerializeField] float wanderRadius;
    [SerializeField] float wanderDistance;
    [SerializeField] float wanderJitter;
    [SerializeField] float minDetectionBoxLength; // obstacle
    [SerializeField] float wallDetectionLength; // wall

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
                return Seek(GetComponent<EnemyMover>().targetTr.position);
            case State.Flee:
                return Flee(GetComponent<EnemyMover>().targetTr.position);
            case State.Arrive:
                return Arrive(GetComponent<EnemyMover>().targetTr.position, deceleration);
            case State.Pursuit:
                return Pursuit(GetComponent<EnemyMover>().targetTr.GetComponent<Rigidbody2D>());
            case State.Evade:
                return Evade(GetComponent<EnemyMover>().targetTr.GetComponent<Rigidbody2D>());
            case State.Wander:
                return Wander();
            case State.ObstacleAvoidance:
                return SeekWithObstacleAvoidance(GetComponent<EnemyMover>().targetTr.position);
            case State.WallAvoidance:
                return SeekWithWallAvoidance(GetComponent<EnemyMover>().targetTr.position);
            default:
                return Vector2.zero;

        }

    }

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

        if (Vector2.Dot(ToEvader, selfHeading) > 0 && // 대면하고
            relativeHeading < Mathf.Cos(160f)) // 서로의 방향이 160도를 넘을 시
        {
            // 그냥 evader의 위치로 Seek
            return Seek(evader.transform.position);
        }

        float lookAheadTime = ToEvader.magnitude / (GetComponent<MovingEntity>().maxSpeed + evader.velocity.magnitude);

        return Seek((Vector2)evader.transform.position + evader.velocity * lookAheadTime);
    }

    private Vector2 Evade(Rigidbody2D pursuer)
    {
        Vector2 ToPursuer = pursuer.transform.position - transform.position;

        float lookAheadTime = ToPursuer.magnitude /
            (movingEntity.maxSpeed + pursuer.GetComponent<MovingEntity>().maxSpeed);

        return Flee((Vector2)pursuer.transform.position + pursuer.velocity * lookAheadTime);
    }
    
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
    
    private Vector2 SeekWithObstacleAvoidance(Vector2 TargetPos)
    {
        Vector2 desiredVelocity = (Seek(TargetPos) + ObstacleAvoidance(TargetPos)).normalized * movingEntity.maxSpeed;
        return desiredVelocity - GetComponent<Rigidbody2D>().velocity;
    }

    private Vector2 ObstacleAvoidance(Vector2 TargetPos)
    {
        // Cast DetectionBox
        // Mark All Obstacles within the DetectionBox
        Vector2 direction = TargetPos - (Vector2)transform.position;
        List<RaycastHit2D> results = new List<RaycastHit2D>();
        float boxLength = CalcBoxLength();
        GetComponent<Collider2D>().Cast(direction, GetContactFilter2D("Obstacle"), results, boxLength);

        Matrix4x4 localToWorld = Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.FromToRotation(Vector3.right, direction),
            Vector3.one
            );

        // no obstacle front
        if (results.Count == 0)
        {
            return Seek(TargetPos);
        }
        // getting stucked
        if(lastTimeObstacle != null && GetComponent<Collider2D>().IsTouching(lastTimeObstacle))
        {
            Vector2 perp1 = Vector2.Perpendicular(direction);
            Vector2 perp2 = -1f * perp1;

            Vector2 ToObstacle = lastTimeObstacle.transform.position - transform.position;
            return Vector2.Dot(perp1, ToObstacle) < 0f ?
                perp1.normalized * movingEntity.maxSpeed :
                perp2.normalized * movingEntity.maxSpeed;
        }

        // Find a Nearest Obstacle
        Collider2D nearestIntersectingObstacle = GetNearestIntersectingObstacle(results);
        lastTimeObstacle = nearestIntersectingObstacle; // cache it for getting stucked situation

        // Calculate SteeringLocalForce
        Vector2 steeringForceLocal = CalcSteeringLocalForce(boxLength, nearestIntersectingObstacle, localToWorld);

        // Project LocalForce To World
        Vector2 steeringForceWorld = localToWorld.MultiplyPoint3x4(steeringForceLocal);

        return steeringForceWorld;
    }

    private Vector2 SeekWithWallAvoidance(Vector2 TargetPos)
    {
        Vector2 desiredVelocity = (Seek(TargetPos) + WallAvoidance(TargetPos)).normalized * movingEntity.maxSpeed;
        return desiredVelocity - GetComponent<Rigidbody2D>().velocity;
    }


    [SerializeField] Collider2D detectedWall;
    private Vector2 WallAvoidance(Vector2 TargetPos)
    {
        Vector2 direction = GetComponent<Rigidbody2D>().velocity;
        direction.Normalize();

        // getting stucked situation
        if(detectedWall && GetComponent<Collider2D>().IsTouching(detectedWall))
        {
            List<ContactPoint2D> contactPoints = new List<ContactPoint2D>();
            GetComponent<Collider2D>().GetContacts(contactPoints);
            if(contactPoints.Count > 0)
            {
                return PerpendicularVectorFrom(contactPoints);
            }
        }

        //// three way Raycast ////
        List<RaycastHit2D> results = new List<RaycastHit2D>();

        // to direction
        List<RaycastHit2D> hits2D = new List<RaycastHit2D>();
        if (Physics2D.Raycast(transform.position, direction, GetContactFilter2D("Wall"), hits2D, wallDetectionLength) > 0)
            foreach (var hit in hits2D)
                results.Add(hit);

        // to +30 degree from direction
        Vector2 plus30degree = Quaternion.AngleAxis(30f, Vector3.forward) * direction;
        hits2D.Clear();
        if (Physics2D.Raycast(transform.position, plus30degree, GetContactFilter2D("Wall"), hits2D, wallDetectionLength) > 0)
            foreach (var hit in hits2D)
                results.Add(hit);

        // to -30 degree from direction
        Vector2 minus30degree = Quaternion.AngleAxis(-30f, Vector3.forward) * direction;
        hits2D.Clear();
        if (Physics2D.Raycast(transform.position, minus30degree, GetContactFilter2D("Wall"), hits2D, wallDetectionLength) > 0)
            foreach (var hit in hits2D)
                results.Add(hit);

        // no wall
        if (results.Count == 0)
        {
            return Vector2.zero;
        }

        //// get closest wall ////
        RaycastHit2D closestWallHit = results.OrderBy((r) => Vector2.Distance(transform.position, r.transform.position)).ToList()[0];
        detectedWall = closestWallHit.collider;

        //// generate normal vector ////
        Vector2 normal = closestWallHit.normal;
        float mag = wallDetectionLength - closestWallHit.distance;

        Vector2 steeringForce = normal * mag;
        return steeringForce;
    }


    #region Helper Functions
    private Vector2 PerpendicularVectorFrom(List<ContactPoint2D> contactPoints)
    {
        Vector2 normalFromContact = contactPoints[0].normal;
        Vector2 ToDetectedWall = detectedWall.transform.position - transform.position;

        Vector2 perp1 = Vector2.Perpendicular(normalFromContact);
        Vector2 perp2 = -perp1;

        if (Vector2.Dot(ToDetectedWall, perp1) < 0) return perp1.normalized * movingEntity.maxSpeed;
        else return perp2.normalized * movingEntity.maxSpeed;
    }
    private Vector2 CalcSteeringLocalForce(float boxLength, Collider2D nearestIntersectingObstacle, Matrix4x4 localToWorld)
    {
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


        return new Vector2(steeringLocalForceX, steeringLocalForceY);
    }
    private static float SignReverse(float v) => (-1f) * Mathf.Sign(v);
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
    private ContactFilter2D GetContactFilter2D(params string[] layernames)
    {
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.useLayerMask = true;
        foreach(var name in layernames)
            contactFilter2D.layerMask.value |= LayerMask.GetMask(name);
        
        return contactFilter2D;
    }
    private float CalcBoxLength() => minDetectionBoxLength + (GetComponent<Rigidbody2D>().velocity.magnitude / movingEntity.maxSpeed) * minDetectionBoxLength;
    #endregion


    private void OnDrawGizmos()
    {
        Vector2 direction = GetComponent<Rigidbody2D>().velocity;
        Vector2 plus30degree = Quaternion.AngleAxis(30f, Vector3.forward) * direction;
        Vector2 minus30degree = Quaternion.AngleAxis(-30f, Vector3.forward) * direction;


        Debug.DrawLine(transform.position, (Vector2)transform.position + direction.normalized * wallDetectionLength, Color.red);
        Debug.DrawLine(transform.position, (Vector2)transform.position + plus30degree.normalized * wallDetectionLength, Color.red);
        Debug.DrawLine(transform.position, (Vector2)transform.position + minus30degree.normalized * wallDetectionLength, Color.red);
    }
}

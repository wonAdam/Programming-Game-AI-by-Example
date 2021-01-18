using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class SteeringBehaviors : MonoBehaviour
{
    public enum State
    {
        Seek, Flee, Arrive, Pursuit, Evade, Wander, ObstacleAvoidance, WallAvoidance, Interpose, Hide, FollowPath, OffsetPursuit
    }

    public enum Deceleration
    {
        fast = 1, normal = 2, slow = 3
    }

    private Vector2 wanderTarget;
    private Collider2D lastTimeObstacle;
    public State state = State.Seek;

    [SerializeField] public Deceleration deceleration = Deceleration.slow;
    [SerializeField] float panicDistance = 7f;
    [SerializeField] float wanderRadius;
    [SerializeField] float wanderDistance;
    [SerializeField] float wanderJitter;
    [SerializeField] float minDetectionBoxLength; // obstacle
    [SerializeField] float wallDetectionLength; // wall
    [SerializeField] Collider2D detectedWall;
    [SerializeField] Vector2 prevHidingSpot;
    [SerializeField] public bool hasHidingSpot = false;

    /// <summary>
    /// State에 따른 적절한 다음 MovingEntity에 줘야할 Force를 리턴합니다.
    /// </summary>
    /// <returns></returns>
    public Vector2 Calculate(MovingEntity movingEntity)
    {
        switch (state)
        {
            case State.Seek:
                return Seek(movingEntity.target.position, movingEntity);
            case State.Flee:
                return Flee(movingEntity.target.position, movingEntity);
            case State.Arrive:
                return Arrive(movingEntity.target.position, deceleration);
            case State.Pursuit:
                return Pursuit(movingEntity.target, movingEntity);
            case State.Evade:
                return Evade(movingEntity.target, movingEntity);
            case State.Wander:
                return Wander();
            case State.ObstacleAvoidance:
                return ArriveWithObstacleAvoidance(movingEntity.target.position, movingEntity);
            case State.WallAvoidance:
                return ArriveWithWallAvoidance(GetComponent<MovingEntity>().target.position, movingEntity);
            case State.Interpose:
                return Interpose(GetComponent<AIMover>().agentA.transform, GetComponent<AIMover>().agentB.transform, movingEntity);
            case State.Hide:
                Collider2D[] obstacles = GetAllObstacles();
                return Hide(movingEntity.target, obstacles, movingEntity);
            case State.FollowPath:
                return FollowPath(movingEntity);
            case State.OffsetPursuit:
                return OffsetPursuit(movingEntity);
            default:
                return Vector2.zero;

        }

    }

    public Waypoint destWaypoint = null;

    private Vector2 Seek(Vector2 targetPos, MovingEntity movingEntity)
    {
        Vector2 desiredVelocity = 
            (targetPos - (Vector2)transform.position).normalized * movingEntity.maxSpeed;
        return desiredVelocity - GetComponent<Rigidbody2D>().velocity;
    }

    private Vector2 Flee(Vector2 targetPos, MovingEntity  movingEntity)
    {

        if (Vector2.Distance(targetPos, transform.position) > panicDistance)
        {
            return Vector2.zero;
        }
        Vector2 desiredVelocity = ((Vector2)transform.position - targetPos).normalized * movingEntity.maxSpeed;
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

    private Vector2 Pursuit(Transform target, MovingEntity movingEntity)
    {
        Rigidbody2D evader = target.GetComponent<Rigidbody2D>();
        Vector2 ToEvader = evader.transform.position - transform.position;
        Vector2 evaderHeading = evader.velocity.normalized;
        Vector2 selfHeading = this.GetComponent<Rigidbody2D>().velocity.normalized;
        float relativeHeading = Vector2.Dot(evaderHeading, selfHeading);

        if (Vector2.Dot(ToEvader, selfHeading) > 0 && // 대면하고
            relativeHeading < Mathf.Cos(160f)) // 서로의 방향이 160도를 넘을 시
        {
            // 그냥 evader의 위치로 Seek
            return Seek(evader.transform.position, movingEntity);
        }

        float lookAheadTime = ToEvader.magnitude / (GetComponent<MovingEntity>().maxSpeed + evader.velocity.magnitude);

        return Seek((Vector2)evader.transform.position + evader.velocity * lookAheadTime, movingEntity);
    }

    private Vector2 Evade(Transform target, MovingEntity movingEntity)
    {
        Rigidbody2D pursuer = target.GetComponent<Rigidbody2D>();

        Vector2 ToPursuer = pursuer.transform.position - transform.position;

        float lookAheadTime = ToPursuer.magnitude /
            (movingEntity.maxSpeed + pursuer.GetComponent<MovingEntity>().maxSpeed);

        return Flee((Vector2)pursuer.transform.position + pursuer.velocity * lookAheadTime, movingEntity);
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
    
    private Vector2 ArriveWithObstacleAvoidance(Vector2 TargetPos, MovingEntity movingEntity)
    {
        Vector2 desiredVelocity = (Arrive(TargetPos, deceleration) + ObstacleAvoidance(TargetPos, movingEntity)).normalized * movingEntity.maxSpeed;
        return desiredVelocity - GetComponent<Rigidbody2D>().velocity;
    }

    private Vector2 ObstacleAvoidance(Vector2 TargetPos, MovingEntity movingEntity)
    {
        // Cast DetectionBox
        // Mark All Obstacles within the DetectionBox
        Vector2 direction = TargetPos - (Vector2)transform.position;
        List<RaycastHit2D> results = new List<RaycastHit2D>();
        float boxLength = CalcBoxLength(movingEntity);
        GetComponent<Collider2D>().Cast(direction, GetContactFilter2D("Obstacle"), results, boxLength);

        Matrix4x4 localToWorld = Matrix4x4.TRS(
            Vector3.zero,
            Quaternion.FromToRotation(Vector3.right, direction),
            Vector3.one
            );

        // no obstacle front
        if (results.Count == 0)
        {
            return Seek(TargetPos, movingEntity);
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

    private Vector2 ArriveWithWallAvoidance(Vector2 TargetPos, MovingEntity movingEntity)
    {
        Vector2 desiredVelocity = (Arrive(TargetPos, deceleration) + WallAvoidance(TargetPos, movingEntity)).normalized * movingEntity.maxSpeed;
        return desiredVelocity - GetComponent<Rigidbody2D>().velocity;
    }
    
    private Vector2 WallAvoidance(Vector2 TargetPos, MovingEntity movingEntity)
    {
        Vector2 direction = GetComponent<Rigidbody2D>().velocity;
        direction.Normalize();

        // getting stucked situation
        if (detectedWall && GetComponent<Collider2D>().IsTouching(detectedWall))
        {
            List<ContactPoint2D> contactPoints = new List<ContactPoint2D>();
            GetComponent<Collider2D>().GetContacts(contactPoints);
            if (contactPoints.Count > 0)
            {
                return PerpendicularVectorFrom(contactPoints, movingEntity);
            }
        }

        //// three way Raycast ////
        List<RaycastHit2D> results = new List<RaycastHit2D>();
        ThreeWayRaycastTowardDirection(direction, results);

        // no wall
        if (results.Count == 0)
            return Vector2.zero;

        //// get closest wall ////
        RaycastHit2D closestWallHit = results.OrderBy((r) => Vector2.Distance(transform.position, r.transform.position)).ToList()[0];
        detectedWall = closestWallHit.collider;

        //// generate normal vector ////
        Vector2 normal = closestWallHit.normal;
        float mag = wallDetectionLength - closestWallHit.distance;

        Vector2 steeringForce = normal * mag;
        return steeringForce;
    }

    private Vector2 Interpose(Transform AgentA, Transform AgentB, MovingEntity movingEntity)
    {
        Vector2 midPoint = (AgentA.transform.position - AgentB.transform.position) / 2.0f;

        float timeToReachMidPoint = (midPoint - (Vector2)transform.position).magnitude / movingEntity.maxSpeed;

        Vector2 aPos = (Vector2)AgentA.position + AgentA.GetComponent<Rigidbody2D>().velocity * timeToReachMidPoint;
        Vector2 bPos = (Vector2)AgentB.position + AgentB.GetComponent<Rigidbody2D>().velocity * timeToReachMidPoint;

        midPoint = (aPos + bPos) / 2.0f;

        return Arrive(midPoint, Deceleration.fast);
    }

    private Vector2 Hide(Transform target, Collider2D[] obstacles, MovingEntity movingEntity)
    {
        // Check if you are in enemy sight or too close
        // inside sight angle -30degree ~ +30degree
        bool insideSightAngle;
        bool insideSightLength;
        Vector2 enemyFront = target.right;
        Vector2 enemyToPlayer = (transform.position - target.position);
        AmIInsideEnemySightRange(out insideSightAngle, out insideSightLength, enemyToPlayer, enemyFront, target.GetComponent<MovingEntity>());

        if (NoNeedToHide(insideSightAngle, enemyToPlayer, insideSightLength)) return Vector2.zero;

        // Decide whether should evade or hide
        Vector2 hidingSpot;
        if (GetHidingSpot(target.position, obstacles, out hidingSpot))
        {
            hasHidingSpot = true;
            if (Vector2.Distance(prevHidingSpot, hidingSpot) > 0.5f)
            {
                prevHidingSpot = hidingSpot;
                return ArriveWithObstacleAvoidance(hidingSpot, movingEntity);
            }
            else
            {
                return ArriveWithObstacleAvoidance(prevHidingSpot, movingEntity);
            }
        }
        else
        {
            hasHidingSpot = false;
            return Evade(target, movingEntity);
        }

    }

    private Vector2 FollowPath(MovingEntity movingEntity)
    {
        if (movingEntity.path == null) return Vector2.zero;

        if (destWaypoint == null) // pick nearest
            destWaypoint = movingEntity.path.GetNearestWaypoint(transform.position);

        // goto destination waypoint
        if (Vector2.Distance(destWaypoint.transform.position, transform.position) < 0.4f)
        {
            Waypoint nextDestWaypoint = movingEntity.path.GetNextWaypoint(destWaypoint);
            if (nextDestWaypoint != null)
            {
                destWaypoint = nextDestWaypoint;
                return Arrive(destWaypoint.transform.position, deceleration);
            }
            else
            {
                return Arrive(destWaypoint.transform.position, deceleration);
            }
        }
        else
        {
            return Arrive(destWaypoint.transform.position, deceleration);
        }
    }

    private Vector2 OffsetPursuit(MovingEntity movingEntity)
    {
        Matrix4x4 leaderSpaceToWorldSpace = Matrix4x4.TRS(
                movingEntity.leader.transform.position - Vector3.zero,
                Quaternion.FromToRotation(Vector3.right, movingEntity.leader.transform.right),
                Vector3.one
                );
        Vector2 desiredPos = leaderSpaceToWorldSpace.MultiplyPoint3x4(movingEntity.localOffsetInLeaderSpace);

        Vector2 ToOffset = desiredPos - (Vector2)transform.position;
        float lookAheadTime = ToOffset.magnitude / (movingEntity.maxSpeed + movingEntity.leader.maxSpeed);
        return Arrive(desiredPos + movingEntity.leader.GetComponent<Rigidbody2D>().velocity * lookAheadTime, Deceleration.fast);
    }


    #region Helper Functions

    private static Collider2D[] GetAllObstacles() => FindObjectsOfType<Collider2D>().Where((c) => (1 << c.gameObject.layer & LayerMask.GetMask("Obstacle")) > 0).ToArray();
    private void AmIInsideEnemySightRange(out bool insideSightAngle, out bool insideSightLength, Vector2 enemyToPlayer, Vector2 enemyFront, MovingEntity target)
    {
        insideSightAngle = false;
        if (Vector2.Dot(enemyFront.normalized, enemyToPlayer.normalized) > Mathf.Cos(target.sightAngle * Mathf.Deg2Rad) &&
            Vector2.Dot(enemyFront.normalized, enemyToPlayer.normalized) > Mathf.Cos(-target.sightAngle * Mathf.Deg2Rad))
            insideSightAngle = true;

        insideSightLength = false;
        if (enemyToPlayer.magnitude <= target.sightLength * 2f)
            insideSightLength = true;
    }
    private bool NoNeedToHide(bool insideSightAngle, Vector2 enemyToPlayer, bool insideSightLength)
    {
        return !hasHidingSpot && enemyToPlayer.magnitude > 5.5f && !(insideSightAngle && insideSightLength);
    }

    /// <summary>
    ///  Get Hiding Position Based on Multiple Obstacles
    /// </summary>
    /// <returns>True: if you should evade / False: if you get bestHidingSpot</returns>
    private bool GetHidingSpot(Vector2 TargetPos, Collider2D[] obstacles, out Vector2 bestHidingSpot)
    {
        float distanceToClosest = float.MaxValue;

        bestHidingSpot = new Vector2();

        foreach (var obstacle in obstacles)
        {
            Vector2 hidingSpot = GetHidingPosition(obstacle.transform.position, obstacle.bounds.size.x / 2f, TargetPos);

            float dist = Vector2.Distance(hidingSpot, transform.position);

            if (dist < distanceToClosest)
            {
                distanceToClosest = dist;
                bestHidingSpot = hidingSpot;
            }
        }

        if (bestHidingSpot == (Vector2)transform.position || distanceToClosest == float.MaxValue)
            return false;

        return true;
    }
    /// <summary>
    /// Get Hinding Position Based on One Obstacle
    /// </summary>
    private Vector2 GetHidingPosition(Vector2 obPos, float obRadius, Vector2 TargetPos)
    {
        const float distFromOb = 0.2f;
        float distAway = obRadius + distFromOb;

        Vector2 toOb = obPos - TargetPos;

        return (toOb.normalized * distAway) + obPos;
    }
    private void ThreeWayRaycastTowardDirection(Vector2 direction, List<RaycastHit2D> results)
    {
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
    }
    private Vector2 PerpendicularVectorFrom(List<ContactPoint2D> contactPoints, MovingEntity movingEntity)
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
    private float CalcBoxLength(MovingEntity movingEntity) => minDetectionBoxLength + (GetComponent<Rigidbody2D>().velocity.magnitude / movingEntity.maxSpeed) * minDetectionBoxLength;
    #endregion


    private void OnDrawGizmos()
    {
        if(state == State.WallAvoidance)
        {
            Vector2 direction = GetComponent<Rigidbody2D>().velocity;
            Vector2 plus30degree = Quaternion.AngleAxis(30f, Vector3.forward) * direction;
            Vector2 minus30degree = Quaternion.AngleAxis(-30f, Vector3.forward) * direction;

            Debug.DrawLine(transform.position, (Vector2)transform.position + direction.normalized * wallDetectionLength, Color.red);
            Debug.DrawLine(transform.position, (Vector2)transform.position + plus30degree.normalized * wallDetectionLength, Color.red);
            Debug.DrawLine(transform.position, (Vector2)transform.position + minus30degree.normalized * wallDetectionLength, Color.red);
        }

        else
        {
            Vector2 direction = GetComponent<Rigidbody2D>().velocity;
            Debug.DrawLine(transform.position, (Vector2)transform.position + direction, Color.red);
        }
    }
}

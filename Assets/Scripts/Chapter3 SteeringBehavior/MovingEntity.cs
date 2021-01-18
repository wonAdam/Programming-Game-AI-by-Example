using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingEntity : MonoBehaviour
{
    [SerializeField] public float maxSpeed;
    [SerializeField] public float maxForce;
    [SerializeField] public float maxTurnRate;
    [SerializeField] public Transform target;
    [SerializeField] public Path path;


    [Header("Only For Seeker in Hide Mode")]
    [SerializeField] public float sightAngle;
    [SerializeField] public float sightLength;


    [Header("Only For non-leader in Offset Pursuit Mode")]
    [SerializeField] public MovingEntity leader;
    public Vector2 localOffsetInLeaderSpace;

    public virtual void Start()
    {
        SetOffsetFromLeader();
    }

    public void SetOffsetFromLeader()
    {
        Matrix4x4 worldSpaceToLeaderSpace = Matrix4x4.TRS(
                Vector3.zero - leader.transform.position,
                Quaternion.FromToRotation(leader.transform.right, Vector3.right),
                Vector3.one
                );

        localOffsetInLeaderSpace = worldSpaceToLeaderSpace.MultiplyPoint3x4(transform.position);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEntity : MonoBehaviour
{
    [SerializeField] public float maxSpeed;
    [SerializeField] public float maxForce;
    [SerializeField] public float maxTurnRate;
    [SerializeField] public Transform target;
    [SerializeField] public Path path;


    [Header("Only For Seeker in Hide Mode")]
    [SerializeField] public float sightAngle;
    [SerializeField] public float sightLength;
}

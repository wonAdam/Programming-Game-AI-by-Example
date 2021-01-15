using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEntity : MonoBehaviour
{
    [SerializeField] public float maxSpeed;
    [SerializeField] public float maxForce;
    [SerializeField] public float maxTurnRate;
    [SerializeField] public Transform targetTr;

}

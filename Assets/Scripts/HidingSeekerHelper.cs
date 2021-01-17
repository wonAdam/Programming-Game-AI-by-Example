using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(MovingEntity))]
public class HidingSeekerHelper : MonoBehaviour
{
    [SerializeField] MovingEntity movingEntity;
    private void OnDrawGizmos()
    {
        if(!movingEntity)
            movingEntity = GetComponent<MovingEntity>();

        Vector2 rightAngleSight = Quaternion.Euler(0f, 0f, movingEntity.sightAngle) * transform.right * movingEntity.sightLength * 2f;
        Debug.DrawLine(transform.position, transform.position + (Vector3)rightAngleSight, Color.red);
        Vector2 leftAngleSight = Quaternion.Euler(0f, 0f, -movingEntity.sightAngle) * transform.right * movingEntity.sightLength * 2f;
        Debug.DrawLine(transform.position, transform.position + (Vector3)leftAngleSight, Color.red);
    }
}

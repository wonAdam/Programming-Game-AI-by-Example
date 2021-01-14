using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MatrixTransformation : MonoBehaviour
{

    [SerializeField] Vector2 direction;
    [SerializeField] Transform target;
    [SerializeField] Transform nextLocalDirection;
    private void OnDrawGizmos()
    {
        Vector3 worldXBasis = Vector3.right;
        Vector3 worldYBasis = Vector3.up;
        Vector3 worldZBasis = Vector3.forward;
        Debug.DrawLine(Vector3.zero,Vector3.right,Color.red);
        Debug.DrawLine(Vector3.zero,Vector3.up,Color.green);
        Debug.DrawLine(Vector3.zero,Vector3.forward,Color.blue);

        Vector3 localXBasis = (Vector2)transform.position + direction;
        Vector3 localYBasis = (Vector2)transform.position + Vector2.Perpendicular(direction);
        Vector3 localZBasis = transform.position + Vector3.Cross(transform.position + (Vector3)direction, transform.position + (Vector3)Vector2.Perpendicular(direction));
        Debug.DrawLine(transform.position, localXBasis, Color.red);
        Debug.DrawLine(transform.position, localYBasis, Color.green);
        Debug.DrawLine(transform.position, localZBasis, Color.blue);

        // vector from local to target
        Debug.DrawLine(transform.position, target.position, Color.white);

        Matrix4x4 ToLocal = 
            Matrix4x4.TRS(
                transform.position, 
                Quaternion.FromToRotation(Vector3.right, direction), 
                Vector3.one);

        Vector2 trsTarget = ToLocal.MultiplyPoint3x4(target.position);
        Debug.DrawLine(Vector2.zero, trsTarget, Color.red);


        // basis transformation world to local
        Debug.DrawLine(transform.position, ToLocal.MultiplyPoint3x4(Vector3.right), Color.cyan);
        Debug.DrawLine(transform.position, ToLocal.MultiplyPoint3x4(Vector3.up), Color.cyan);
        Debug.DrawLine(transform.position, ToLocal.MultiplyPoint3x4(Vector3.forward), Color.cyan);

        Debug.DrawLine(Vector2.zero, ToLocal.inverse.MultiplyPoint3x4(target.position), 
                Color.white);

        Debug.DrawLine(transform.position, nextLocalDirection.position, Color.gray);
        Debug.DrawLine(Vector3.zero, nextLocalDirection.position - transform.position, Color.gray);
    }
}


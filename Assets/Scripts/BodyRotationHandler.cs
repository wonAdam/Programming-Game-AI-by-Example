using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyRotationHandler : MonoBehaviour
{
    void Update()
    {
        if (GetComponent<Rigidbody2D>().velocity.magnitude < Mathf.Epsilon) return;
        Quaternion rot = Quaternion.FromToRotation(transform.right, GetComponent<Rigidbody2D>().velocity);
        
        transform.rotation = rot * transform.rotation;
    }
}

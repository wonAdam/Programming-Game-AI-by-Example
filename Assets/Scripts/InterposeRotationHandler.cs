using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterposeRotationHandler : MonoBehaviour
{
    void Update()
    {
        Quaternion rot = Quaternion.FromToRotation(transform.right, GetComponent<Rigidbody2D>().velocity);
        transform.rotation = rot * transform.rotation;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterposeRotationHandler : MonoBehaviour
{
    void Update()
    {
        Quaternion rot = Quaternion.FromToRotation(transform.right, GetComponent<Rigidbody2D>().velocity);
        if(rot.eulerAngles.z > 4f)
            transform.rotation = rot * transform.rotation;
    }
}

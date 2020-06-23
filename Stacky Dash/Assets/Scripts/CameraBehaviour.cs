using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Transform target;
    public float zOffset;
    public float heightOffset;
    public float speed;
    private Vector3 newPos=Vector3.zero;
    private Vector3 vel = Vector3.zero;

    
    void FixedUpdate()
    {
        if (target != null)//for safety
        {
            newPos.y = target.position.y + heightOffset;
            newPos.z = target.position.z - zOffset;
            newPos.x = target.position.x/2;
            transform.position = Vector3.SmoothDamp(transform.position, newPos, ref vel, speed * Time.deltaTime);
            
        }
    }
}

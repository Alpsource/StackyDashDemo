using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownRotator : MonoBehaviour
{
    private Vector3 rotationVec = new Vector3(0, 0, 2);
    void Update()
    {
        transform.Rotate(rotationVec);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMono : MonoBehaviour
{
    public Vector3 Direction;
    public void Move(float dT)
    {
        transform.position += Direction * dT;
    }
}

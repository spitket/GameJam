using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop_Rotator : MonoBehaviour
{
    public float rotationSpeed = 180f;
    
    private Coroutine tiltCoroutine;
    
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ch_CharacterController : MonoBehaviour
{
    public float rotationSpeed = 100.0f; 
    private float rotationY = 0.0f; 
    public float smoothVelocity = 100.0f; 
    public float smoothTime = 0.001f; 
    
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        
        float rotationAmount = h * rotationSpeed * Time.deltaTime;
        rotationY = Mathf.SmoothDamp(rotationY, rotationY + rotationAmount, ref smoothVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0, rotationY, 0);
    }

}

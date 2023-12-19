using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mng_LightManager : MonoBehaviour
{
    public Light light1;
    public Light light2;
    public Light light3;
    
    public float timer = 1f;
    private void Start()
    {
        StartCoroutine(LightControlRoutine());
    }

    IEnumerator LightControlRoutine()
    {
        while (true)
        {
            light1.enabled = true;
            light2.enabled = false;
            light3.enabled = false;
            yield return new WaitForSeconds(timer);

            light1.enabled = false;
            light2.enabled = true;
            light3.enabled = false;
            yield return new WaitForSeconds(timer);
            
            light1.enabled = false;
            light2.enabled = false;
            light3.enabled = true;
            yield return new WaitForSeconds(timer);
        }
    }


}

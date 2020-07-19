using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScrips : MonoBehaviour
{
    int rotationspeed = 1;

    //void start()
    //{
        
    //}
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0,rotationspeed,0);
        rotationspeed += 1;
    }
}

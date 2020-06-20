using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProbePosition : MonoBehaviour
{

    [SerializeField] private Camera VRCam;
    Vector3 pos;

    void Start()
    {
        pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
         pos.x = VRCam.transform.position.x;
         pos.z = VRCam.transform.position.z;

        transform.position = pos;

    }
}

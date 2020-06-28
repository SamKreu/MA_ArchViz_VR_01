using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class ProfilingData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Profiler.logFile = "mylog";
        //Profiler.enableBinaryLog = true;
        //Profiler.enabled = true;

        StartCoroutine(ProfilingCoroutine());
    }

    IEnumerator ProfilingCoroutine()
    {
        yield return new WaitForSeconds(8);
        Profiler.enabled = false;
    }

}

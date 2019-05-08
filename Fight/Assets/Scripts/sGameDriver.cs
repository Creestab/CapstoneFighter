using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class sGameDriver : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.targetFrameRate != 60) { Application.targetFrameRate = 60; }
    }
}

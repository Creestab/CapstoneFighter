using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sMoveData : MonoBehaviour
{
    //List of moves.

    public static Dictionary<string, float[,]> FrameData = new Dictionary<string, float[,]>()
    {
        //Jabs
        {"jab1", new float[,]{{0,10}, {2,5}}}
    };
    
    public static Dictionary<string, float[,]> GetFrameData
    {
        get { return FrameData; }
    }
}

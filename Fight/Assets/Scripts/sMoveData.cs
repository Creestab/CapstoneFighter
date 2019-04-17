using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sMoveData : MonoBehaviour
{
    //List of moves
    /*{{frame 0, ending frame, # of hit instances, special frame type (0 if none), special frame start, special frame end, rarity (for draft: 0 common, 1 uncommon, 2 rare)},
            {hitbox start frame, hitbox end frame, damage, knockback angle, knockback base, knockback growth, hitstun frames}, ... } */
    public static Dictionary<string, float[,]> FrameData = new Dictionary<string, float[,]>()
    {
        //Jabs
        {"Vanilla Punch", new float[,]{{0,10,1,0,0,0,0}, {2,5,3,0,10,1,1}}}
    };
    
    public static Dictionary<string, float[,]> GetFrameData
    {
        get { return FrameData; }
    }
}

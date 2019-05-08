using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _sMoveData : MonoBehaviour
{
    //List of moves
    /*{{frame 0, ending frame, # of hit instances, special frame type (0 if none), special effect value (0 if none), special frame start, special frame end, rarity (for draft: 0 common, 1 uncommon, 2 rare)},
            {hitbox start frame, hitbox end frame, damage, knockback angle, knockback base, knockback growth, hitstun frames, priority}, ... } */
    public static Dictionary<string, float[,]> FrameData = new Dictionary<string, float[,]>()
    {
        //Jabs
        {"Vanilla Punch", new float[,]{{0,10,1,0,0,0,0,0}, {2,5,3,0,10,1,1,0}}},

        //Up Lights
        {"Vanilla Fist Pump", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Forward Lights
        {"Vanilla Arm Swipe", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Down Lights
        {"Vanilla Stomp", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Up Strongs
        {"Skyclap", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Forward Strongs
        {"This Is Sparta", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Down Strongs
        {"The Splits", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Neutral Aerials
        {"Sex Kick", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Up Aerials
        {"Bicycle Kick", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Forward Aerials
        {"Swat Away", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Down Aerials
        {"Sky Stomp", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Back Aerials
        {"Boneless", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Neutral Specials
        {"Twirl", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Up Specials
        {"Skycutter Leap", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Forward Specials
        {"Bullrush", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Down Specials
        {"The Worm", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}}
    };
    
    public static Dictionary<string, float[,]> GetFrameData
    {
        get { return FrameData; }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sBattleDriver : MonoBehaviour
{
    float tFPS;
    Text guiFR;
    Text guiT;
    Text guiP1S;
    Text guiP2S;

    public int tSpawn;

    sGameDriver.Player p1;
    sGameDriver.Player p2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setP1(sGameDriver.Player p)
    {
        p1 = p;
    }
    public void setP2(sGameDriver.Player p)
    {
        p2 = p;
    }
}

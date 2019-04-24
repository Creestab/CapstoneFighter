using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class sGameDriver : MonoBehaviour
{
    public struct Player
    {
        public string name;
        public GameObject model;
        public Color color;
        public sPlayer character;
        public sInput controller;
        public Vector3 spawnpoint;
        public int stocks;

        Player(string n, GameObject mdl, Color col, sPlayer pChar, sInput ctrl, Vector3 sp, int s)
        {
            name = n;
            model = mdl;
            color = col;
            character = pChar;
            controller = ctrl;
            spawnpoint = sp;
            stocks = s;
        }
    }

    public GameObject pfPlayer;

    sStartSettings startup;
    sBattleDriver fight;

    public int tRespawn;
    public int maxStocks;

    //Player 1 Data
    public string p1Name;
    public Color p1Color;
    public TextAsset p1ControlScheme;
    //Player 2 Data
    public string p2Name;
    public Color p2Color;
    public TextAsset p2ControlScheme;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        startup = new sStartSettings();
        fight = new sBattleDriver();

        fight.setP1(new Player(p1Name, pfPlayer, p1Color, pfPlayer.GetComponent<sPlayer>, pfPlayer.GetComponent<sInput>, new Vector3(-14f, 7.5f, -2f), 3));
        fight.setP2(new Player(p2Name, pfPlayer, p2Color, pfPlayer.GetComponent<sPlayer>, pfPlayer.GetComponent<sInput>, new Vector3(14f, 7.5f, -2f), 3));
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.targetFrameRate != 60) { Application.targetFrameRate = 60; }


    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //game variables
    public int respawn;

    //Player prefab
    public GameObject player;

    //Player 1 variables
    private GameObject p1;
    private PlayerController p1Script;
    public Vector3 p1Spawn;
    public string p1Left = "a";
    public string p1Right = "d";
    public string p1Up = "w";
    public string p1Down = "s";
    public string p1Jump = "space";
    public string p1Atk = "m";
    public string p1Spec = "k";
    public string p1Block = "l";
    public string p1Grab = ";";
    public Color p1Color = Color.red;

    //Player 2 variables
    private GameObject p2;
    private PlayerController p2Script;
    public Vector3 p2Spawn;
    public string p2Left = "left";
    public string p2Right = "right";
    public string p2Up = "up";
    public string p2Down = "down";
    public string p2Jump = "right ctrl";
    public string p2Atk = "1";
    public string p2Spec = "5";
    public string p2Block = "6";
    public string p2Grab = "enter";
    public Color p2Color = Color.blue;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;
        respawn = respawn * Application.targetFrameRate;

        p1Spawn = new Vector3(-12f, 7.5f, -2f);
        p2Spawn = new Vector3(12f, 7.5f, -2f);

        p1 = (GameObject)Instantiate(player, p1Spawn, Quaternion.Euler(0, 0, 0));
        p2 = (GameObject)Instantiate(player, p2Spawn, Quaternion.Euler(0, 0, 0));
        p1Script = p1.GetComponent<PlayerController>();
        p2Script = p1.GetComponent<PlayerController>();

        //For control inputs, see https://docs.unity3d.com/Manual/ConventionalGameInput.html
        //
        //Player 1 initialization
        p1.GetComponent<Renderer>().material.color = p1Color;
        p1Script.ctrlLeft = p1Left;
        p1Script.ctrlRight = p1Right;
        p1Script.ctrlUp = p1Up;
        p1Script.ctrlDown = p1Down;
        p1Script.ctrlJump = p1Jump;
        p1Script.ctrlAtk = p1Atk;
        p1Script.ctrlSpec = p1Spec;
        p1Script.ctrlBlock = p1Block;
        p1Script.ctrlGrab = p1Grab;
        p1Script.playerNum = 1;
        p1Script.spawnPoint = p1Spawn;
        p1Script.moveSpeed = 100;
        p1Script.jumpSpeedGrounded = 100;
        p1Script.jumpSpeedAirborn = 100;
        p1Script.numAirJumps = 1;
        p1Script.inputBuffer = 3;
        //Player 2 initialization
        p1.GetComponent<Renderer>().material.color = p1Color;
        p2Script.ctrlLeft = p1Left;
        p2Script.ctrlRight = p1Right;
        p2Script.ctrlUp = p1Up;
        p2Script.ctrlDown = p1Down;
        p2Script.ctrlJump = p1Jump;
        p2Script.ctrlAtk = p1Atk;
        p2Script.ctrlSpec = p1Spec;
        p2Script.ctrlBlock = p1Block;
        p2Script.ctrlGrab = p1Grab;
        p2Script.playerNum = 2;
        p2Script.spawnPoint = p2Spawn;
        p2Script.moveSpeed = 100;
        p2Script.jumpSpeedGrounded = 100;
        p2Script.jumpSpeedAirborn = 100;
        p2Script.numAirJumps = 1;
        p2Script.inputBuffer = 3;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Update is called on frame interval
    void FixedUpdate()
    {

    }
}

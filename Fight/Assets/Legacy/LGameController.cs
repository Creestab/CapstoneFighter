using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
public class GameController : MonoBehaviour
{
    //game variables
    public int respawn;     //Respawn time for players
    public int stockCount;  //Starting stock count for players
    private int p1Stocks;   //Player 1's current stocks
    private int p2Stocks;   //Player 2's current stocks
    private float t;        //Used to calculate FPS

    //Game assets/prefabs
    public GameObject player;
    private Text guiFramerate;
    private Text guiTimer;
    private Text guiP1Stocks;
    private Text guiP2Stocks;

    //For control inputs, see https://docs.unity3d.com/Manual/ConventionalGameInput.html
    //
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
    public string p1Name = "Player 1";

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
    public string p2Name = "Player 2";

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        p1Stocks = stockCount;
        p2Stocks = stockCount;

        p1Spawn = new Vector3(-14f, 7.5f, -2f);
        p2Spawn = new Vector3(14f, 7.5f, -2f);

        StartGUI();

        StartCoroutine("SpawnP1");
        StartCoroutine("SpawnP2");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGUI();
    }

    // Update is called on frame interval
    void FixedUpdate()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>().playerNum == 1)
        {
            p1Stocks--;
            if (p1Stocks > 0)
            {
                Destroy(p1);
                StartCoroutine("SpawnP1");
            }
            else { Victory(p2); }
        }
        if (collision.gameObject.GetComponent<PlayerController>().playerNum == 2)
        {
            p2Stocks--;
            if (p2Stocks > 0)
            {
                Destroy(p2);
                StartCoroutine("SpawnP2");
            }
            else { Victory(p1); }
        }
    }

    private void StartGUI()
    {
        guiFramerate = GameObject.Find("Framerate").GetComponent<Text>();
        guiTimer = GameObject.Find("Timer").GetComponent<Text>();
        guiP1Stocks = GameObject.Find("Player1Stocks").GetComponent<Text>();
        guiP2Stocks = GameObject.Find("Player2Stocks").GetComponent<Text>();

        GameObject.Find("P1StockLabel").GetComponent<Text>().text = p1Name;
        GameObject.Find("P1StockLabel").GetComponent<Text>().color = p1Color;
        guiP1Stocks.text = stockCount.ToString();
        guiP1Stocks.color = p1Color;

        GameObject.Find("P2StockLabel").GetComponent<Text>().text = p2Name;
        GameObject.Find("P2StockLabel").GetComponent<Text>().color = p2Color;
        guiP2Stocks.text = stockCount.ToString();
        guiP2Stocks.color = p2Color;
    }

    private void UpdateGUI()
    {
        int minutes = Mathf.FloorToInt(Time.time / 60F);
        int seconds = Mathf.FloorToInt(Time.time - (minutes * 60));
        guiTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        guiP1Stocks.text = p1Stocks.ToString();
        guiP2Stocks.text = p2Stocks.ToString();

        t += (Time.deltaTime - t) * 0.1f;
        float fps = 1.0f / t;
        guiFramerate.text = Mathf.Ceil(fps).ToString();
    }

    IEnumerator Spawn(GameObject obj, Vector3 loc, Quaternion rot, float spawnIn, float spawnOut)
    {
        yield return new WaitForSeconds(spawnIn);
        if (spawnOut <= 0)
        {
            Instantiate(obj,loc,rot);
            StopCoroutine("Spawn");
        }
        else
        {
            GameObject temp = (GameObject)Instantiate(obj, loc, rot);
            Destroy(temp, spawnOut);
            StopCoroutine("Spawn");
        }
    }

    IEnumerator SpawnP1()
    {
        if (p1Stocks == stockCount) {yield return new WaitForSeconds(0);}
        else {yield return new WaitForSeconds(respawn);}

        p1 = (GameObject)Instantiate(player, p1Spawn, Quaternion.Euler(0, 0, 0));
        p1Script = p1.GetComponent<PlayerController>();
        p1.GetComponent<Renderer>().material.color = p1Color;

        //Player 1 initialization
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

        StopCoroutine("SpawnP1");
    }

    IEnumerator SpawnP2()
    {
        if (p2Stocks == stockCount) { yield return new WaitForSeconds(0); }
        else { yield return new WaitForSeconds(respawn); }

        p2 = (GameObject)Instantiate(player, p2Spawn, Quaternion.Euler(0, 0, 0));
        p2Script = p2.GetComponent<PlayerController>();
        p2.GetComponent<Renderer>().material.color = p2Color;

        //Player 2 initialization
        p2Script.ctrlLeft = p2Left;
        p2Script.ctrlRight = p2Right;
        p2Script.ctrlUp = p2Up;
        p2Script.ctrlDown = p2Down;
        p2Script.ctrlJump = p2Jump;
        p2Script.ctrlAtk = p2Atk;
        p2Script.ctrlSpec = p2Spec;
        p2Script.ctrlBlock = p2Block;
        p2Script.ctrlGrab = p2Grab;

        p2Script.playerNum = 2;
        p2Script.spawnPoint = p2Spawn;
        p2Script.moveSpeed = 100;
        p2Script.jumpSpeedGrounded = 100;
        p2Script.jumpSpeedAirborn = 100;
        p2Script.numAirJumps = 1;
        p2Script.inputBuffer = 3;
        StopCoroutine("SpawnP2");
    }

    public void Victory(GameObject winner)
    {

    }
}
*/
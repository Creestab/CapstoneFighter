using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class sBattleDriver : MonoBehaviour
{
    public GameObject Player;
    int frame;
    float tFPS;
    Text guiFR;
    Text guiT;
    Text guiP1SL;
    Text guiP1SC;
    Text guiP2SL;
    Text guiP2SC;

    //Match settings
    public int respawnTime;
    public int startingStocks;
    public string player1Name;
    public Color player1Color;
    public TextAsset player1Controls;
    public TextAsset player1Moveset;
    public string player2Name;
    public Color player2Color;
    public TextAsset player2Controls;
    public TextAsset player2Moveset;

    //Player 1 Variables
    private GameObject p1;
    private static Vector3 p1Spawn;
    private int p1Stocks;
    
    //Player 2 Variables
    private GameObject p2;
    private Vector3 p2Spawn;
    private int p2Stocks;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        frame = 0;

        p1Spawn = new Vector3(-14f, 7.5f, -2f);
        p2Spawn = new Vector3(14f, 7.5f, -2f);

        p1Stocks = startingStocks;
        p2Stocks = startingStocks;

        StartGUI();

        StartCoroutine("SpawnP1");
        StartCoroutine("SpawnP2");
    }

    // Update is called once per frame
    void Update()
    {
        frame++;
        UpdateGUI();

        //DEBUGGING
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            Debug.Log("P1 Log @F" + frame);
            Debug.Log(p1.GetComponent<sPlayer>().ToString() + '\n' + '\n');
            Debug.Log(p1.GetComponent<sInput>().ToString() + '\n' + '\n');
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Blastzone Triggered");
        if (other.material.name == "pmPlayer")
        {
            Debug.Log("Player Collision with Blastzone");
            if (other.material)
            {
                p1Stocks--;
                if (p1Stocks > 0)
                {
                    Destroy(p1);
                    StartCoroutine("SpawnP1");
                }
                else { }
            }
            if (other.gameObject.GetComponent<sPlayer>().pNumber == 2)
            {
                p2Stocks--;
                if (p2Stocks > 0)
                {
                    Destroy(p2);
                    StartCoroutine("SpawnP2");
                }
                else { }
            }
        }
    }

    IEnumerator SpawnP1()
    {
        if (p1Stocks == startingStocks) { yield return new WaitForSeconds(0); }
        else { yield return new WaitForSeconds(respawnTime); }

        p1 = Instantiate(Player, p1Spawn, Quaternion.Euler(0, 0, 0));
        p1.GetComponent<sPlayer>().Spawn(1, player1Moveset.name, player1Controls.name, player1Color, 1);
        p1.GetComponent<sInput>().Refresh();

        //Print Player 1 state on creation
        Debug.Log("Battle Driver P1 TextAsset: " + player1Controls.name + ", " + player1Moveset.name);
        Debug.Log(p1.GetComponent<sPlayer>().ToString());
        Debug.Log(p1.GetComponent<sInput>().ToString());

        StopCoroutine("SpawnP1");
    }

    IEnumerator SpawnP2()
    {
        if (p2Stocks == startingStocks) { yield return new WaitForSeconds(0); }
        else { yield return new WaitForSeconds(respawnTime); }

        p2 = Instantiate(Player, p2Spawn, Quaternion.Euler(0, 0, 0));
        p2.GetComponent<sPlayer>().Spawn(2, player2Moveset.name, player2Controls.name, player2Color, -1);
        p2.GetComponent<sInput>().Refresh();

        //Print Player 2 state on creation
        //Debug.Log("Battle Driver P2 TextAsset: " + player2Controls.name + ", " + player2Moveset.name);
        //Debug.Log(p2.GetComponent<sPlayer>().ToString());
        //Debug.Log(p2.GetComponent<sInput>().ToString());

        StopCoroutine("SpawnP2");
    }

    private void StartGUI()
    {
        guiT = GameObject.Find("Timer").GetComponent<Text>();
        guiFR = GameObject.Find("Framerate").GetComponent<Text>();
        guiP1SL = GameObject.Find("P1StockLabel").GetComponent<Text>();
        guiP2SL = GameObject.Find("P1StockLabel").GetComponent<Text>();
        guiP1SC = GameObject.Find("Player1Stocks").GetComponent<Text>();
        guiP2SC = GameObject.Find("Player2Stocks").GetComponent<Text>();

        guiP1SL.text = player1Name;
        guiP1SL.color = player1Color;
        guiP1SC.text = p1Stocks.ToString();
        guiP1SC.color = player1Color;

        guiP2SL.text = player2Name;
        guiP2SL.color = player2Color;
        guiP2SC.text = p2Stocks.ToString();
        guiP2SC.color = player2Color;

        Debug.Log("GUI Initialised");
    }

    private void UpdateGUI()
    {
        int minutes = Mathf.FloorToInt(Time.time / 60F);
        int seconds = Mathf.FloorToInt(Time.time - (minutes * 60));
        guiT.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        guiP1SC.text = p1Stocks.ToString();
        guiP2SC.text = p2Stocks.ToString();

        tFPS += (Time.deltaTime - tFPS) * 0.1f;
        float fps = 1.0f / tFPS;
        guiFR.text = Mathf.Ceil(fps).ToString();
    }
}

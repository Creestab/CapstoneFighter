using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sBattleDriver : MonoBehaviour
{
    public GameObject Player;

    float tFPS;
    Text guiFR;
    Text guiT;
    Text guiP1S;
    Text guiP2S;

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
        UpdateGUI();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<sPlayer>().pNumber == 1)
        {
            p1Stocks--;
            if (p1Stocks > 0)
            {
                Destroy(p1);
                StartCoroutine("SpawnP1");
            }
            else { }
        }
        if (collision.gameObject.GetComponent<sPlayer>().pNumber == 2)
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

    IEnumerator SpawnP1()
    {
        if (p1Stocks == startingStocks) { yield return new WaitForSeconds(0); }
        else { yield return new WaitForSeconds(respawnTime); }

        p1 = Instantiate(Player, p1Spawn, Quaternion.Euler(0, 0, 0));
        p1.GetComponent<Renderer>().material.color = player1Color;
        p1.GetComponent<sInput>().SetControls(player1Controls);
        p1.GetComponent<sPlayer>().SetPlayerMoves(player1Moveset);
        p1.GetComponent<sPlayer>().pNumber = 1;
        p1.GetComponent<sPlayer>().moveSpeed = 100;
        p1.GetComponent<sPlayer>().jumpSpeed = 100;
        p1.GetComponent<sPlayer>().airjumpSpeed = 100;
        p1.GetComponent<sPlayer>().maxJumps = 2;
        p1.GetComponent<sPlayer>().orientation = 1;

        StopCoroutine("SpawnP1");
    }

    IEnumerator SpawnP2()
    {
        if (p2Stocks == startingStocks) { yield return new WaitForSeconds(0); }
        else { yield return new WaitForSeconds(respawnTime); }

        p2 = Instantiate(Player, p2Spawn, Quaternion.Euler(0, 0, 0));
        p2.GetComponent<Renderer>().material.color = player2Color;
        p2.GetComponent<sInput>().SetControls(player2Controls);
        p2.GetComponent<sPlayer>().SetPlayerMoves(player2Moveset);
        p2.GetComponent<sPlayer>().pNumber = 2;
        p2.GetComponent<sPlayer>().moveSpeed = 100;
        p2.GetComponent<sPlayer>().jumpSpeed = 100;
        p2.GetComponent<sPlayer>().airjumpSpeed = 100;
        p2.GetComponent<sPlayer>().maxJumps = 2;
        p2.GetComponent<sPlayer>().orientation = -1;

        StopCoroutine("SpawnP2");
    }

    private void StartGUI()
    {
        guiFR = GameObject.Find("Framerate").GetComponent<Text>();
        guiT = GameObject.Find("Timer").GetComponent<Text>();
        guiP1S = GameObject.Find("Player1Stocks").GetComponent<Text>();
        guiP2S = GameObject.Find("Player2Stocks").GetComponent<Text>();

        GameObject.Find("P1StockLabel").GetComponent<Text>().text = player1Name;
        GameObject.Find("P1StockLabel").GetComponent<Text>().color = player1Color;
        guiP1S.text = p1Stocks.ToString();
        guiP1S.color = player1Color;

        GameObject.Find("P2StockLabel").GetComponent<Text>().text = player2Name;
        GameObject.Find("P2StockLabel").GetComponent<Text>().color = player2Color;
        guiP2S.text = p2Stocks.ToString();
        guiP2S.color = player2Color;
    }

    private void UpdateGUI()
    {
        int minutes = Mathf.FloorToInt(Time.time / 60F);
        int seconds = Mathf.FloorToInt(Time.time - (minutes * 60));
        guiT.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        guiP1S.text = p1Stocks.ToString();
        guiP2S.text = p2Stocks.ToString();

        tFPS += (Time.deltaTime - tFPS) * 0.1f;
        float fps = 1.0f / tFPS;
        guiFR.text = Mathf.Ceil(fps).ToString();
    }
}

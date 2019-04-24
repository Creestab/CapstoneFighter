using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSensorData
{
    [SerializeField] sSensor _sensors;
    [SerializeField] sPlayer.enumMoves _atkColliders;

    public sSensor GetSensors { get { return _sensors; } }
    public sPlayer.enumMoves GetAtkColliders { get { return _atkColliders; } }
}

public class sPlayer : MonoBehaviour
{
    [SerializeField] Animator _anim;
    [SerializeField] List<PlayerSensorData> _sensors;
    [SerializeField] Dictionary<enumMoves, MoveData> _moves;

    public GameObject pChar;
    public TextAsset ctrlProfile;

    public int pNumber;
    public int orientation; //1 for facing right, -1 for facing left.
    bool airborne;
    bool fastfall;
    public static int maxJumps;
    int jumps;

    float mHorz;
    float mVert;

    // Start is called before the first frame update
    void Start()
    {
        airborne = true;
        fastfall = false;
        jumps = maxJumps - 1;

        mVert = 0;
        mHorz = 0;
    }

    private void Update()
    {
        if(orientation == 1)
        {
            gameObject.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if(orientation == -1)
        {
            gameObject.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    public struct MoveData
    {
        public enumMoves moveSlot;
        public string moveName;
        public float[,] frameData;

        public MoveData(enumMoves slot, string name)
        {
            moveSlot = slot;
            moveName = name;
            frameData = sMoveData.FrameData[name];
        }
    }
    public enum enumMoves
    {
        none,
        jump,
        jab,
        uLight,
        fLight,
        dLight,
        uStrong,
        fStrong,
        dStrong,
        nAir,
        uAir,
        fAir,
        dAir,
        bAir,
        nSpec,
        uSpec,
        fSpec,
        dSpec,
        grab,
        pummel,
        uThrow,
        fThrow,
        dThrow,
        bThrow,
        getup,
        getupAtk,
        shield,
        fRoll,
        bRoll,
        dodge,
        airdodge
    }

    public Animator GetCharAnimator { get { return _anim; } }
    public List<PlayerSensorData> GetPlayerSensors { get { return _sensors; } }
    public Dictionary<enumMoves, MoveData> GetPlayerMoves {  get { return _moves; } }

    public bool isAirborne()
    {
        return airborne;
    }
    public void modAirborne()
    {
        airborne = !airborne;
    }
}

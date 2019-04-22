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

    public int pNumber;
    public int orientation; //1 for facing right, -1 for facing left.
    public bool airborne;

    // Start is called before the first frame update
    void Start()
    {

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
}

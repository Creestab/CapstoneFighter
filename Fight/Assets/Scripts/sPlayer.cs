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
        jab,
        uTilt,
        fTilt,
        dTilt,
        uStrong,
        fStrong,
        dStrong,
        uAir,
        fAir,
        dAir,
        bAir,
        grab,
        pummel,
        uThrow,
        fThrow,
        dThrow,
        bThrow,
        getupAtk
    }

    public Animator GetCharAnimator { get { return _anim; } }
    public List<PlayerSensorData> GetPlayerSensors { get { return _sensors; } }
    public Dictionary<enumMoves, MoveData> GetPlayerMoves {  get { return _moves; } }
}

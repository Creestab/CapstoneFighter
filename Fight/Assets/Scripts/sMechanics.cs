using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSensorData
{
    [SerializeField] sSensor _sensors;
    [SerializeField] sMechanics.enumAttacks _atkColliders;

    public sSensor GetSensors { get{return _sensors;} }
    public sMechanics.enumAttacks GetAtkColliders { get{return _atkColliders;} }
}

public class sMechanics : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [SerializeField] float _value;
    [SerializeField] Animator _anim;

    List<PlayerSensorData> _pSensors;

    public enum enumAttacks
    {
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

    public float GetSliderVal
    {
        get { return _value; }
        set { _value = value; }
    }
    public Animator GetCharAnimator { get{return _anim;} }
    public List<PlayerSensorData> getPlayerSensors { get{return _pSensors;} }
}
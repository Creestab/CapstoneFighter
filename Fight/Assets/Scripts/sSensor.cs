using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sSensor : MonoBehaviour
{
    [SerializeField] GameObject _player;
    [SerializeField] sData.ColliderState _type;
    [SerializeField] string _moveName;
    [SerializeField] int _hitNum;
    float[,] moveData;

    Vector3 pos;
    Quaternion rot;

    void Start()
    {
        if (_type == sData.ColliderState.HurtBox) { _moveName = null; _hitNum = 0; moveData = null; }
        else moveData = sData.GetMove(_moveName);

        pos = transform.localPosition;
        rot = transform.localRotation;
    }

    private void Update()
    {
        transform.localPosition = pos;
        transform.localRotation = rot;
    }

    public GameObject getPlayer { get { return _player; } }
    public sData.ColliderState getColliderType
    {
        get { return _type; }
        set { _type = value; }
    }
    public string getMoveName { get { return _moveName; } }
    public float[,] getMoveData { get { return moveData; } }
    public int getHitNumber { get { return _hitNum; } }

    private void OnTriggerEnter(Collider other)
    {
        if (_type == sData.ColliderState.HurtBox)
        {
           
        }
    }
}

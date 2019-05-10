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

    public sData.ColliderState GetColliderType
    {
        get { return _type; }
        set { _type = value; }
    }
    public float[,] GetMoveData { get { return moveData; } }

    private void OnTriggerEnter(Collider other)
    {
        if (_type == sData.ColliderState.HurtBox)
        {
            if (other.material.name == "pmHitbox" && other.GetComponent<sSensor>().GetColliderType == sData.ColliderState.HitBox)
            {                
                //Process hit
                sSensor hit = other.GetComponent<sSensor>();
                sPlayer plr = _player.GetComponent<sPlayer>();
                sPlayer opp = hit._player.GetComponent<sPlayer>();

                //Damage
                plr.ModDamage(moveData[hit._hitNum, 2]);
                Debug.Log("Player " + plr.pNumber + " takes " + moveData[hit._hitNum, 2] + " damage from Player " + opp.pNumber + "'s " + hit._moveName);

                //Hitstun


                //Knockback

            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_type == sData.ColliderState.HurtBox)
        {
            if (other.material.name.Contains("pmWall") && !_player.GetComponent<sPlayer>().OnStage())
            {
                if (_player.transform.position.x > 0)
                {
                    if(_player.transform.position.x < 20) _player.transform.Translate(new Vector3(1,0,0), Space.World);
                }
                else if(_player.transform.position.x > -20)
                {
                    _player.transform.Translate(new Vector3(-1, 0, 0), Space.World);
                }
            }
        }
    }
}

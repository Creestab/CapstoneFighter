using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sSensor : MonoBehaviour
{
    [SerializeField] sUtil.ColliderState _type;
    [SerializeField] GameObject _player;
    [SerializeField] string _move;

    Vector3 pos;
    Quaternion rot;

    void Start()
    {
        pos = transform.localPosition;
        rot = transform.localRotation;
    }

    private void Update()
    {
        transform.localPosition = pos;
        transform.localRotation = rot;
    }

    public sUtil.ColliderState GetColliderType
    {
        get { return _type; }
        set { _type = value; }
    }
    public GameObject GetPlayer
    {
        get { return _player; }
    }
    public string GetMove
    {
        get { return _move; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_type == sUtil.ColliderState.HurtBox)
        {
            if (other.material.name == "pmHitbox" && other.GetComponent<sSensor>().GetColliderType == sUtil.ColliderState.HitBox)
            {
                //Process hit

            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_type == sUtil.ColliderState.HurtBox)
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

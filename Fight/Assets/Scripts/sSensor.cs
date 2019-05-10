using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sSensor : MonoBehaviour
{
    void Start()
    {

    }

    [SerializeField] sUtil.ColliderState _type;
    [SerializeField] GameObject _player;
    [SerializeField] string _move;

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
        if (_type == sUtil.ColliderState.HurtBox && other.gameObject.GetComponent<sSensor>().GetColliderType == sUtil.ColliderState.HitBox)
        {
            //Process hit
        }
    }
}

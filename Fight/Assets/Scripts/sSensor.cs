using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sSensor : MonoBehaviour
{
    public enum ColliderState
    {
        None,
        HitBox,
        HurtBox
    };

    void Start()
    {

    }

    [SerializeField] ColliderState _instance;
    [SerializeField] Transform _parent;
    public ColliderState GetColliderState
    {
        get { return _instance; }
        set { _instance = value; }
    }
    public Transform GetParent
    {
        get { return _parent; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<sSensor>().GetColliderState == ColliderState.HitBox)
        {
            var tempScript = other.GetComponent<sSensor>();
            if (_parent != tempScript.GetParent)
            {
                Debug.Log(_parent.name);
                Debug.Log(other.name);
                Debug.Log(this.name);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sInput : MonoBehaviour
{
    [SerializeField] sMechanics sMech;

    public KeyCode moveRight;

    // Start is called before the first frame update
    void Start()
    {
        sMech = this.GetComponent<sMechanics>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(moveRight))
        {
            sMech.GetCharAnimator.SetTrigger("WalkRight");
        }
    }
}

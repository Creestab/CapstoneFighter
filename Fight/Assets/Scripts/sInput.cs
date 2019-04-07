using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sInput : MonoBehaviour
{
    [SerializeField] sMechanics sMech;

    public KeyCode iMoveRight;
    public KeyCode iTilt;

    // Start is called before the first frame update
    void Start()
    {
        sMech = this.GetComponent<sMechanics>();
    }

    // Update is called once per frame
    void Update()
    {
        //Tilt action
        if (Input.GetKeyDown(iTilt))
        {
            if (Input.GetKey(iMoveRight))   { sMech.GetCharAnimator.SetTrigger("TiltRight"); }
            else                            { sMech.GetCharAnimator.SetTrigger("Jab"); }
        }
        //No action inputs
        else if (Input.GetKeyDown(iMoveRight))
        {
            sMech.GetCharAnimator.SetTrigger("WalkRight");
        }
    }
}

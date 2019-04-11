using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sInput : MonoBehaviour
{
    [SerializeField] sPlayer pControl;

    public KeyCode iJump;
    public KeyCode iMoveRight;
    public KeyCode iTilt;

    // Start is called before the first frame update
    void Start()
    {
        pControl = this.GetComponent<sPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Is the character in an actable state
        if (pControl.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || pControl.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Airborne"))
        {
            if (Input.GetKeyDown(iJump))
            {
                if (pControl.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    pControl.GetCharAnimator.Play("JumpSquat");
                }
                else if (pControl.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Airborne"))
                {
                    pControl.GetCharAnimator.Play("AirJump");
                }
            }
            //Tilt action
            else if (Input.GetKeyDown(iTilt))
            {
                if (Input.GetKey(iMoveRight)) { pControl.GetCharAnimator.Play("TiltRight"); }
                else { pControl.GetCharAnimator.Play("Jab"); }
            }
            //No action inputs
            else if (Input.GetKeyDown(iMoveRight))
            {
                pControl.GetCharAnimator.Play("WalkRight");
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sInput : MonoBehaviour
{
    [SerializeField] sPlayer sMech;

    public KeyCode iJump;
    public KeyCode iMoveRight;
    public KeyCode iTilt;

    // Start is called before the first frame update
    void Start()
    {
        sMech = this.GetComponent<sPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Is the character in an actable state
        if (sMech.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || sMech.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Airborne"))
        {
            if (Input.GetKeyDown(iJump))
            {
                if (sMech.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    sMech.GetCharAnimator.Play("JumpSquat");
                }
                else if (sMech.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Airborne"))
                {
                    sMech.GetCharAnimator.Play("AirJump");
                }
            }
            //Tilt action
            else if (Input.GetKeyDown(iTilt))
            {
                if (Input.GetKey(iMoveRight)) { sMech.GetCharAnimator.Play("TiltRight"); }
                else { sMech.GetCharAnimator.Play("Jab"); }
            }
            //No action inputs
            else if (Input.GetKeyDown(iMoveRight))
            {
                sMech.GetCharAnimator.Play("WalkRight");
            }
        }
    }
}

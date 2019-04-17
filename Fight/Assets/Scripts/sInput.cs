using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class sInput : MonoBehaviour
{
    public enum InputAction { Move, Light, Heavy, Special, Block, Grab, Jump, Alt, Pause };
    public struct ControlScheme
    {
        public int buffer;
        public string moveHorz;
        public string moveVert;
        public KeyCode left;
        public KeyCode right;
        public KeyCode up;
        public KeyCode down;
        public KeyCode light;
        public KeyCode heavy;
        public float lightToHeavy; //Threshold for light attack inputs to register as heavy. Values over 1 disable this.
        public KeyCode special;
        public KeyCode block;
        public KeyCode grab;
        public KeyCode jump;
        public KeyCode alt;
        public KeyCode pause;
    }

    sPlayer pChar;
    ControlScheme controls;

    bool actable;
    int stun;

    sPlayer.enumMoves qInput;
    int xBuf;
    bool forceHeavy;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        pChar = transform.parent.GetComponent<sPlayer>();

        qInput = sPlayer.enumMoves.none;
        xBuf = 0;
        forceHeavy = false;
    }

    private void FixedUpdate()
    {

        if (xBuf == 0) { qInput = sPlayer.enumMoves.none; } else if (xBuf > 0) { xBuf--; }

        if (!actable)
        {
            //Is the player facing right?
            if (pChar.orientation == 1)
            {
                //Is the player on the ground?
                if (!pChar.airborne)
                {
                    //Is the player inputting a light attack?
                    if (Input.GetKeyDown(controls.light))
                    {
                        //Is it a shield grab?
                        if (Input.GetKey(controls.block)) { qInput = sPlayer.enumMoves.grab; }
                        //Is it a below the heavy attack threshold?
                        else if (Input.GetAxis(controls.moveHorz) < controls.lightToHeavy && Input.GetAxis(controls.moveVert) < controls.lightToHeavy)
                        {
                            //Is it not a jab?
                            if (Input.GetAxis(controls.moveHorz) != 0 || Input.GetAxis(controls.moveVert) != 0)
                            {
                                //What type of tilt?
                                if (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > Mathf.Abs(Input.GetAxis(controls.moveVert)))
                                {
                                    //Forward Tilt
                                    if (Input.GetAxis(controls.moveHorz) > 0) { qInput = sPlayer.enumMoves.fTilt; }
                                    //Pivot Forward Tilt
                                    else
                                    {
                                        pChar.orientation = -pChar.orientation;
                                        qInput = sPlayer.enumMoves.fTilt;
                                    }
                                }
                                else
                                {
                                    //Up Tilt
                                    if (Input.GetAxis(controls.moveVert) > 0) { qInput = sPlayer.enumMoves.uTilt; }
                                    //Down Tilt
                                    else { qInput = sPlayer.enumMoves.dTilt; }
                                }
                            }
                            else //Its a jab
                            {
                                qInput = sPlayer.enumMoves.jab;
                            }
                        }
                        else
                        {
                            forceHeavy = true;
                        }
                    }
                    //Is the player inputting a roll?
                    else if (Input.GetKeyDown(controls.block))
                    {
                        //Roll right
                        if (Input.GetAxis(controls.moveHorz) > .5) { qInput = sPlayer.enumMoves.fRoll; }
                        //Roll left
                        if (Input.GetAxis(controls.moveHorz) < -.5) { qInput = sPlayer.enumMoves.bRoll; }
                    }
                    //Is the player inputting a special?
                    else if (Input.GetKeyDown(controls.special))
                    {
                        //Is it not a neutral special?
                        if (Input.GetAxis(controls.moveHorz) != 0 || Input.GetAxis(controls.moveVert) != 0)
                        {
                            //What type of special?
                            if (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > Mathf.Abs(Input.GetAxis(controls.moveVert)))
                            {
                                //Forward special
                                if (Input.GetAxis(controls.moveHorz) > 0) { qInput = sPlayer.enumMoves.fSpec; }
                                //Pivot Forward special
                                else
                                {
                                    pChar.orientation = -pChar.orientation;
                                    qInput = sPlayer.enumMoves.fSpec;
                                }
                            }
                            else
                            {
                                //Up special
                                if (Input.GetAxis(controls.moveVert) > 0) { qInput = sPlayer.enumMoves.uSpec; }
                                //Down special
                                else { qInput = sPlayer.enumMoves.dSpec; }
                            }
                        }
                        else //Its a neutral special
                        {
                            qInput = sPlayer.enumMoves.nSpec;
                        }
                    }
                    else if (forceHeavy || Input.GetKeyDown(controls.heavy))
                    {

                    }
                    else if (Input.GetKeyDown(controls.grab)) { }
                    else if (Input.GetKeyDown(controls.jump)) { }
                }
                //The player is airborne
                else
                {

                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Is the character in an actable state
        if (pChar.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || pChar.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Airborne"))
        {
            if (Input.GetKeyDown(controls.jump))
            {
                if (pChar.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    pChar.GetCharAnimator.Play("JumpSquat");
                }
                else if (pChar.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Airborne"))
                {
                    pChar.GetCharAnimator.Play("AirJump");
                }
            }
            //Tilt action
            else if (Input.GetKeyDown(controls.light))
            {
                if (Input.GetAxis(controls.moveHorz) <.7) { pChar.GetCharAnimator.Play("TiltRight"); }
                else { pChar.GetCharAnimator.Play("Jab"); }
            }
            //No action inputs
            else if (Input.GetAxis(controls.moveHorz) > .2)
            {
                pChar.GetCharAnimator.Play("WalkRight");
            }
        }
    }
}

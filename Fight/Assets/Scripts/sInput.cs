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
        public InputAction rStickUse;
        public string rHorz;
        public string rVert;
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
        //Clear buffered actions
        if (qInput != sPlayer.enumMoves.none)
        {
            if (xBuf == 0) { qInput = sPlayer.enumMoves.none; } else { xBuf--; }
        }
        sPlayer.enumMoves qTemp = qInput;
        
        ////////////////
        //Input buffer//
        ////////////////

        //Is the player facing right?
        if (pChar.orientation == 1)
        {
            //Is the player on the ground?
            if (!pChar.airborne)
            {
                //Is the player inputting a light attack?
                if (Input.GetKeyDown(controls.light) || (controls.rStickUse == InputAction.Light && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0)))
                {
                    //Is it a shield grab?
                    if (Input.GetKey(controls.block) && Input.GetAxis(controls.rHorz) < .1 && Input.GetAxis(controls.rVert) < .1)
                    {
                        qInput = sPlayer.enumMoves.grab;
                    }
                    //Is it a below the heavy attack threshold?
                    else if (Input.GetAxis(controls.moveHorz) < controls.lightToHeavy && Input.GetAxis(controls.moveVert) < controls.lightToHeavy)
                    {
                        //Is it not a jab?
                        if (Input.GetAxis(controls.moveHorz) != 0 || Input.GetAxis(controls.moveVert) != 0)
                        {
                            //What type of light?
                            if (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > Mathf.Abs(Input.GetAxis(controls.moveVert)))
                            {
                                //Forward light
                                if (Input.GetAxis(controls.moveHorz) > 0) { qInput = sPlayer.enumMoves.fLight; }
                                //Pivot Forward light
                                else
                                {
                                    pChar.orientation = -pChar.orientation;
                                    qInput = sPlayer.enumMoves.fLight;
                                }
                            }
                            else
                            {
                                //Up light
                                if (Input.GetAxis(controls.moveVert) > 0) { qInput = sPlayer.enumMoves.uLight; }
                                //Down light
                                else { qInput = sPlayer.enumMoves.dLight; }
                            }
                        }
                        else //Its a jab
                        {
                            qInput = sPlayer.enumMoves.jab;
                        }
                    }
                    //Handling of c-stick input
                    else if (controls.rStickUse == InputAction.Light && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0))
                    {
                        //What type of light?
                        if (Mathf.Abs(Input.GetAxis(controls.rHorz)) > Mathf.Abs(Input.GetAxis(controls.rVert)))
                        {
                            //Forward light
                            if (Input.GetAxis(controls.rHorz) > 0) { qInput = sPlayer.enumMoves.fLight; }
                            //Pivot Forward light
                            else
                            {
                                pChar.orientation = -pChar.orientation;
                                qInput = sPlayer.enumMoves.fLight;
                            }
                        }
                        else
                        {
                            //Up light
                            if (Input.GetAxis(controls.rVert) > 0) { qInput = sPlayer.enumMoves.uLight; }
                            //Down light
                            else { qInput = sPlayer.enumMoves.dLight; }
                        }
                    }
                    else
                    {
                        forceHeavy = true;
                    }
                }
                //Is the player inputting a roll/dodge?
                else if (Input.GetKeyDown(controls.block))
                {
                    //Roll right
                    if (Input.GetAxis(controls.moveHorz) > .5) { qInput = sPlayer.enumMoves.fRoll; }
                    //Roll left
                    else if (Input.GetAxis(controls.moveHorz) < -.5) { qInput = sPlayer.enumMoves.bRoll; }
                    else if (Input.GetAxis(controls.moveVert) < -.5) { qInput = sPlayer.enumMoves.dodge; }
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
                            if (Input.GetAxis(controls.moveVert) > 0)
                            {
                                if (Input.GetAxis(controls.moveHorz) < -.1) { pChar.orientation = -pChar.orientation; }
                                qInput = sPlayer.enumMoves.uSpec;
                            }
                            //Down special
                            else { qInput = sPlayer.enumMoves.dSpec; }
                        }
                    }
                    else //Its a neutral special
                    {
                        qInput = sPlayer.enumMoves.nSpec;
                    }
                }
                else if (forceHeavy || Input.GetKeyDown(controls.heavy) || (controls.rStickUse == InputAction.Heavy && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0)))
                {
                    //Is it non-directional?
                    if ((Input.GetKeyDown(controls.heavy) || forceHeavy) && (Input.GetAxis(controls.moveHorz) != 0 || Input.GetAxis(controls.moveVert) != 0))
                    {
                        //What type of strong?
                        if (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > Mathf.Abs(Input.GetAxis(controls.moveVert)))
                        {
                            //Forward strong
                            if (Input.GetAxis(controls.moveHorz) > 0) { qInput = sPlayer.enumMoves.fStrong; }
                            //Pivot Forward strong
                            else
                            {
                                pChar.orientation = -pChar.orientation;
                                qInput = sPlayer.enumMoves.fStrong;
                            }
                        }
                        else
                        {
                            //Up strong
                            if (Input.GetAxis(controls.moveVert) > 0) { qInput = sPlayer.enumMoves.uStrong; }
                            //Down strong
                            else { qInput = sPlayer.enumMoves.dStrong; }
                        }
                    }
                    //Handling of c-stick input
                    else if (controls.rStickUse == InputAction.Heavy && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0))
                    {
                        //What type of strong?
                        if (Mathf.Abs(Input.GetAxis(controls.rHorz)) > Mathf.Abs(Input.GetAxis(controls.rVert)))
                        {
                            //Forward strong
                            if (Input.GetAxis(controls.rHorz) > 0) { qInput = sPlayer.enumMoves.fStrong; }
                            //Pivot Forward strong
                            else
                            {
                                pChar.orientation = -pChar.orientation;
                                qInput = sPlayer.enumMoves.fStrong;
                            }
                        }
                        else
                        {
                            //Up strong
                            if (Input.GetAxis(controls.rVert) > 0) { qInput = sPlayer.enumMoves.uStrong; }
                            //Down strong
                            else { qInput = sPlayer.enumMoves.dStrong; }
                        }
                    }
                    else //Dedicated strong button with no direction, default to forward strong
                    {
                        qInput = sPlayer.enumMoves.fStrong;
                    }
                }
                else if (Input.GetKeyDown(controls.grab)) { qInput = sPlayer.enumMoves.grab; }
                else if (Input.GetKeyDown(controls.jump)) { qInput = sPlayer.enumMoves.jump; }
            }
            //The player is airborne
            else
            {
                //Is the player inputting an attack?
                if (Input.GetKeyDown(controls.light) || Input.GetKeyDown(controls.heavy) ||
                    ((controls.rStickUse == InputAction.Light || controls.rStickUse == InputAction.Heavy) && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0)))
                {
                    //Is it not a neautral air?
                    if ((Input.GetKeyDown(controls.light) || Input.GetKeyDown(controls.heavy)) && (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > .25 || Mathf.Abs(Input.GetAxis(controls.moveVert)) > .25))
                    {
                        //What type of aerial?
                        if (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > Mathf.Abs(Input.GetAxis(controls.moveVert)))
                        {
                            //Forward air
                            if (Input.GetAxis(controls.moveHorz) > 0) { qInput = sPlayer.enumMoves.fAir; }
                            //back air
                            else { qInput = sPlayer.enumMoves.bAir; }
                        }
                        else
                        {
                            //Up air
                            if (Input.GetAxis(controls.moveVert) > 0) { qInput = sPlayer.enumMoves.uAir; }
                            //Down light
                            else { qInput = sPlayer.enumMoves.dAir; }
                        }
                    }
                    else if (Input.GetKeyDown(controls.light) || Input.GetKeyDown(controls.heavy))//Its it a nair
                    {
                        qInput = sPlayer.enumMoves.nAir;
                    }
                    else //c-stick handling
                    {
                        //What type of aerial?
                        if (Mathf.Abs(Input.GetAxis(controls.rHorz)) > Mathf.Abs(Input.GetAxis(controls.rVert)))
                        {
                            //Forward air
                            if (Input.GetAxis(controls.rHorz) > 0) { qInput = sPlayer.enumMoves.fAir; }
                            //Back air
                            else { qInput = sPlayer.enumMoves.bAir; }
                        }
                        else
                        {
                            //Up air
                            if (Input.GetAxis(controls.rVert) > 0) { qInput = sPlayer.enumMoves.uAir; }
                            //Down air
                            else { qInput = sPlayer.enumMoves.dAir; }
                        }
                    }
                }
                //Is the player inputting an airdodge?
                else if (Input.GetKeyDown(controls.block) || Input.GetKeyDown(controls.grab)) { qInput = sPlayer.enumMoves.airdodge; }
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
                            if (Input.GetAxis(controls.moveVert) > 0)
                            {
                                if (Input.GetAxis(controls.moveHorz) < -.1) { pChar.orientation = -pChar.orientation; }
                                qInput = sPlayer.enumMoves.uSpec;
                            }
                            //Down special
                            else { qInput = sPlayer.enumMoves.dSpec; }
                        }
                    }
                    else //Its a neutral special
                    {
                        qInput = sPlayer.enumMoves.nSpec;
                    }
                }
                else if (Input.GetKeyDown(controls.jump)) { qInput = sPlayer.enumMoves.jump; }
            }
        }
        //The player is facing left
        else
        {
            //Is the player on the ground?
            if (!pChar.airborne)
            {
                //Is the player inputting a light attack?
                if (Input.GetKeyDown(controls.light) || (controls.rStickUse == InputAction.Light && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0)))
                {
                    //Is it a shield grab?
                    if (Input.GetKey(controls.block) && Input.GetAxis(controls.rHorz) < .1 && Input.GetAxis(controls.rVert) < .1)
                    {
                        qInput = sPlayer.enumMoves.grab;
                    }
                    //Is it a below the heavy attack threshold?
                    else if (Input.GetAxis(controls.moveHorz) < controls.lightToHeavy && Input.GetAxis(controls.moveVert) < controls.lightToHeavy)
                    {
                        //Is it not a jab?
                        if (Input.GetAxis(controls.moveHorz) != 0 || Input.GetAxis(controls.moveVert) != 0)
                        {
                            //What type of light?
                            if (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > Mathf.Abs(Input.GetAxis(controls.moveVert)))
                            {
                                //Forward light
                                if (Input.GetAxis(controls.moveHorz) < 0) { qInput = sPlayer.enumMoves.fLight; }
                                //Pivot Forward light
                                else
                                {
                                    pChar.orientation = -pChar.orientation;
                                    qInput = sPlayer.enumMoves.fLight;
                                }
                            }
                            else
                            {
                                //Up light
                                if (Input.GetAxis(controls.moveVert) > 0) { qInput = sPlayer.enumMoves.uLight; }
                                //Down light
                                else { qInput = sPlayer.enumMoves.dLight; }
                            }
                        }
                        else //Its a jab
                        {
                            qInput = sPlayer.enumMoves.jab;
                        }
                    }
                    //Handling of c-stick input
                    else if (controls.rStickUse == InputAction.Light && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0))
                    {
                        //What type of light?
                        if (Mathf.Abs(Input.GetAxis(controls.rHorz)) > Mathf.Abs(Input.GetAxis(controls.rVert)))
                        {
                            //Forward light
                            if (Input.GetAxis(controls.rHorz) < 0) { qInput = sPlayer.enumMoves.fLight; }
                            //Pivot Forward light
                            else
                            {
                                pChar.orientation = -pChar.orientation;
                                qInput = sPlayer.enumMoves.fLight;
                            }
                        }
                        else
                        {
                            //Up light
                            if (Input.GetAxis(controls.rVert) > 0) { qInput = sPlayer.enumMoves.uLight; }
                            //Down light
                            else { qInput = sPlayer.enumMoves.dLight; }
                        }
                    }
                    else
                    {
                        forceHeavy = true;
                    }
                }
                //Is the player inputting a roll/dodge?
                else if (Input.GetKeyDown(controls.block))
                {
                    //Roll right
                    if (Input.GetAxis(controls.moveHorz) < -.5) { qInput = sPlayer.enumMoves.fRoll; }
                    //Roll left
                    else if (Input.GetAxis(controls.moveHorz) > .5) { qInput = sPlayer.enumMoves.bRoll; }
                    else if (Input.GetAxis(controls.moveVert) < -.5) { qInput = sPlayer.enumMoves.dodge; }
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
                            if (Input.GetAxis(controls.moveHorz) < 0) { qInput = sPlayer.enumMoves.fSpec; }
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
                            if (Input.GetAxis(controls.moveVert) > 0)
                            {
                                if (Input.GetAxis(controls.moveHorz) > .1) { pChar.orientation = -pChar.orientation; }
                                qInput = sPlayer.enumMoves.uSpec;
                            }
                            //Down special
                            else { qInput = sPlayer.enumMoves.dSpec; }
                        }
                    }
                    else //Its a neutral special
                    {
                        qInput = sPlayer.enumMoves.nSpec;
                    }
                }
                else if (forceHeavy || Input.GetKeyDown(controls.heavy) || (controls.rStickUse == InputAction.Heavy && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0)))
                {
                    //Is it non-directional?
                    if ((Input.GetKeyDown(controls.heavy) || forceHeavy) && (Input.GetAxis(controls.moveHorz) != 0 || Input.GetAxis(controls.moveVert) != 0))
                    {
                        //What type of strong?
                        if (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > Mathf.Abs(Input.GetAxis(controls.moveVert)))
                        {
                            //Forward strong
                            if (Input.GetAxis(controls.moveHorz) < 0) { qInput = sPlayer.enumMoves.fStrong; }
                            //Pivot Forward strong
                            else
                            {
                                pChar.orientation = -pChar.orientation;
                                qInput = sPlayer.enumMoves.fStrong;
                            }
                        }
                        else
                        {
                            //Up strong
                            if (Input.GetAxis(controls.moveVert) > 0) { qInput = sPlayer.enumMoves.uStrong; }
                            //Down strong
                            else { qInput = sPlayer.enumMoves.dStrong; }
                        }
                    }
                    //Handling of c-stick input
                    else if (controls.rStickUse == InputAction.Heavy && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0))
                    {
                        //What type of strong?
                        if (Mathf.Abs(Input.GetAxis(controls.rHorz)) > Mathf.Abs(Input.GetAxis(controls.rVert)))
                        {
                            //Forward strong
                            if (Input.GetAxis(controls.rHorz) < 0) { qInput = sPlayer.enumMoves.fStrong; }
                            //Pivot Forward strong
                            else
                            {
                                pChar.orientation = -pChar.orientation;
                                qInput = sPlayer.enumMoves.fStrong;
                            }
                        }
                        else
                        {
                            //Up strong
                            if (Input.GetAxis(controls.rVert) > 0) { qInput = sPlayer.enumMoves.uStrong; }
                            //Down strong
                            else { qInput = sPlayer.enumMoves.dStrong; }
                        }
                    }
                    else //Dedicated strong button with no direction, default to forward strong
                    {
                        qInput = sPlayer.enumMoves.fStrong;
                    }
                }
                else if (Input.GetKeyDown(controls.grab)) { qInput = sPlayer.enumMoves.grab; }
                else if (Input.GetKeyDown(controls.jump)) { qInput = sPlayer.enumMoves.jump; }
            }
            //The player is airborne
            else
            {
                //Is the player inputting an attack?
                if (Input.GetKeyDown(controls.light) || Input.GetKeyDown(controls.heavy) ||
                    ((controls.rStickUse == InputAction.Light || controls.rStickUse == InputAction.Heavy) && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0)))
                {
                    //Is it not a neautral air?
                    if ((Input.GetKeyDown(controls.light) || Input.GetKeyDown(controls.heavy)) && (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > .25 || Mathf.Abs(Input.GetAxis(controls.moveVert)) > .25))
                    {
                        //What type of aerial?
                        if (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > Mathf.Abs(Input.GetAxis(controls.moveVert)))
                        {
                            //Forward air
                            if (Input.GetAxis(controls.moveHorz) < 0) { qInput = sPlayer.enumMoves.fAir; }
                            //back air
                            else { qInput = sPlayer.enumMoves.bAir; }
                        }
                        else
                        {
                            //Up air
                            if (Input.GetAxis(controls.moveVert) > 0) { qInput = sPlayer.enumMoves.uAir; }
                            //Down light
                            else { qInput = sPlayer.enumMoves.dAir; }
                        }
                    }
                    else if (Input.GetKeyDown(controls.light) || Input.GetKeyDown(controls.heavy))//Its it a nair
                    {
                        qInput = sPlayer.enumMoves.nAir;
                    }
                    else //c-stick handling
                    {
                        //What type of aerial?
                        if (Mathf.Abs(Input.GetAxis(controls.rHorz)) > Mathf.Abs(Input.GetAxis(controls.rVert)))
                        {
                            //Forward air
                            if (Input.GetAxis(controls.rHorz) < 0) { qInput = sPlayer.enumMoves.fAir; }
                            //Back air
                            else { qInput = sPlayer.enumMoves.bAir; }
                        }
                        else
                        {
                            //Up air
                            if (Input.GetAxis(controls.rVert) > 0) { qInput = sPlayer.enumMoves.uAir; }
                            //Down air
                            else { qInput = sPlayer.enumMoves.dAir; }
                        }
                    }
                }
                //Is the player inputting an airdodge?
                else if (Input.GetKeyDown(controls.block) || Input.GetKeyDown(controls.grab)) { qInput = sPlayer.enumMoves.airdodge; }
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
                            if (Input.GetAxis(controls.moveHorz) < 0) { qInput = sPlayer.enumMoves.fSpec; }
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
                            if (Input.GetAxis(controls.moveVert) > 0)
                            {
                                if (Input.GetAxis(controls.moveHorz) > .1) { pChar.orientation = -pChar.orientation; }
                                qInput = sPlayer.enumMoves.uSpec;
                            }
                            //Down special
                            else { qInput = sPlayer.enumMoves.dSpec; }
                        }
                    }
                    else //Its a neutral special
                    {
                        qInput = sPlayer.enumMoves.nSpec;
                    }
                }
                else if (Input.GetKeyDown(controls.jump)) { qInput = sPlayer.enumMoves.jump; }
            }
        }

        //Start frame buffer on a new input
        if (qInput != qTemp) { xBuf = 6; }
    }

    // Update is called once per frame
    void Update()
    {
        //Action processing
        if (actable)
        {
            //Queued input from buffer
            if (qInput != sPlayer.enumMoves.none)
            {
                //Verify actable grounded state
                if (pChar.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || pChar.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
                {
                    //Possible actions
                    if (qInput == sPlayer.enumMoves.jab) { pChar.GetCharAnimator.Play("Jab"); }
                }
                //Verify actable aerial state
                else if (pChar.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Airborne"))
                {

                }
                //Character deemed unactable due to error
                else { actable = false; }

                //Buffered action executed
                qInput = sPlayer.enumMoves.none;
                xBuf = 0;
            }
            //Process non-bufferable actions (like movement)
            else
            {

            }
        }
        //Process influence to inactable states (DI, special fall drift, mashing, ext.)
        else
        {

        }
    }

    public bool isActable()
    {
        return actable;
    }
    public void modActable()
    {
        actable = !actable;
    }

    public int getStun()
    {
        return stun;
    }
    public void setStun(int s)
    {
        stun = s;
    }

    public ControlScheme getControls()
    {
        return controls;
    }
    public void setControls(ControlScheme ctrls)
    {
        controls = ctrls;
    }
    public void setControls(int bfr, string mH, string mV, InputAction rSU, string rH, string rV, KeyCode l, KeyCode r, KeyCode u, KeyCode d,
                            KeyCode lA, KeyCode hA, float l2h, KeyCode s, KeyCode b, KeyCode g, KeyCode j, KeyCode a, KeyCode p)
    {
        controls.buffer = bfr;
        controls.moveHorz = mH;
        controls.moveVert = mV;
        controls.rStickUse = rSU;
        controls.rHorz = rH;
        controls.rVert = rV;
        controls.left = l;
        controls.right = r;
        controls.up = u;
        controls.down = d;
        controls.light = lA;
        controls.heavy = hA;
        controls.lightToHeavy = l2h;
        controls.special = s;
        controls.block = b;
        controls.grab = g;
        controls.jump = j;
        controls.alt = a;
        controls.pause = p;
}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

    sPlayer.enumMoves qInput;
    int xBuf;
    bool forceHeavy;

    int debugFramesFixed;
    int debugFrames;

    // Start is called before the first frame update
    void Start()
    {
        pChar = gameObject.GetComponent<sPlayer>();
        controls = new ControlScheme();

        qInput = sPlayer.enumMoves.none;
        xBuf = 0;
        forceHeavy = false;

        //Debug.Log(debugFramesFixed = 0);
        //Debug.Log(debugFrames = 0);
    }

    private void FixedUpdate()
    {
        debugFramesFixed++;

        //Debug analog sticks
        /*if (Input.GetKeyDown(controls.alt)) { Debug.Log("Printing axis values..."); }
        if (Input.GetKeyDown(controls.alt))
        {
            Debug.Log("   Left Analog Horizontal: " + Input.GetAxis("P1_LHorz"));
            Debug.Log("   Left Analog Vertical: " + Input.GetAxis("P1_LVert"));
            Debug.Log("   Right Analog Horizontal: " + Input.GetAxis("P1_RHorz"));
            Debug.Log("   Right Analog Vertical: " + Input.GetAxis("P1_RVert"));
            Debug.Log("   Left Analog Horizontal: " + Input.GetAxis("P2_LHorz"));
            Debug.Log("   Left Analog Vertical: " + Input.GetAxis("P2_LVert"));
            Debug.Log("   Right Analog Horizontal: " + Input.GetAxis("P2_RHorz"));
            Debug.Log("   Right Analog Vertical: " + Input.GetAxis("P2_RVert"));
        }*/

        //Clear buffered actions
        if (qInput != sPlayer.enumMoves.none)
        {
            if (xBuf == 0)
            {
                qInput = sPlayer.enumMoves.none;
                //Debug.Log("Queued move expired" + "   Fixed Update #" + debugFramesFixed);
            }
            else { xBuf--; }
        }
        sPlayer.enumMoves qTemp = qInput;
        
        ////////////////
        //Input buffer//
        ////////////////

        //Is the player facing right?
        if (pChar.orientation == 1)
        {
            //Is the player on the ground?
            if (!pChar.IsAirborne())
            {
                //Is the player inputting a light attack?
                if (Input.GetKeyDown(controls.light) || (controls.rStickUse == InputAction.Light && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0)))
                {
                    //Is it a shield grab?
                    if (Input.GetKey(controls.block))
                    {
                        qInput = sPlayer.enumMoves.grab;
                    }
                    //Handling of right stick input
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
                        else//Its a jab
                        {
                            qInput = sPlayer.enumMoves.jab;
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
                    //Handling of right stick input
                    if ((controls.rStickUse == InputAction.Light || controls.rStickUse == InputAction.Heavy) && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0))
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
                    //Is it not a neautral air?
                    else if ((Input.GetKeyDown(controls.light) || Input.GetKeyDown(controls.heavy)) && (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > .25 || Mathf.Abs(Input.GetAxis(controls.moveVert)) > .25))
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
                }
                //Is the player inputting airdoge/tech?
                else if (Input.GetKeyDown(controls.block) || Input.GetKeyDown(controls.grab))
                {
                    //What type of tech?
                    if (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > Mathf.Abs(Input.GetAxis(controls.moveVert)))
                    {
                        //Tech Forward Roll
                        if (Input.GetAxis(controls.moveHorz) > 0) { qInput = sPlayer.enumMoves.fRoll; }
                        //Tech Backward Roll
                        else { qInput = sPlayer.enumMoves.bRoll; }
                    }
                    else
                    {
                        //Tech hop
                        if (Input.GetAxis(controls.moveVert) > 0) { qInput = sPlayer.enumMoves.techHop; }
                        //Default to normal tech
                        else { qInput = sPlayer.enumMoves.tech; }
                    }
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
                else if (Input.GetKeyDown(controls.jump) && pChar.CanJump()) { qInput = sPlayer.enumMoves.jump; }
            }
        }
        //The player is facing left
        else
        {
            //Is the player on the ground?
            if (!pChar.IsAirborne())
            {
                //Is the player inputting a light attack?
                if (Input.GetKeyDown(controls.light) || (controls.rStickUse == InputAction.Light && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0)))
                {
                    //Is it a shield grab?
                    if (Input.GetKey(controls.block))
                    {
                        qInput = sPlayer.enumMoves.grab;
                    }
                    //Handling of right stick input
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
                //Is the player inputting a heavy attack?
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
                    //Handling of right stick input
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
                    //Handling of right stick input
                    if ((controls.rStickUse == InputAction.Light || controls.rStickUse == InputAction.Heavy) && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0))
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
                    //Is it not a neautral air?
                    else if ((Input.GetKeyDown(controls.light) || Input.GetKeyDown(controls.heavy)) && (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > .25 || Mathf.Abs(Input.GetAxis(controls.moveVert)) > .25))
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
                }
                //Is the player inputting airdoge/tech?
                else if (Input.GetKeyDown(controls.block) || Input.GetKeyDown(controls.grab))
                {
                    //What type of tech?
                    if (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > Mathf.Abs(Input.GetAxis(controls.moveVert)))
                    {
                        //Tech Back Roll
                        if (Input.GetAxis(controls.moveHorz) > 0) { qInput = sPlayer.enumMoves.bRoll; }
                        //Tech Forward Roll
                        else { qInput = sPlayer.enumMoves.fRoll; }
                    }
                    else
                    {
                        //Tech hop
                        if (Input.GetAxis(controls.moveVert) > 0) { qInput = sPlayer.enumMoves.techHop; }
                        //Default to normal tech
                        else { qInput = sPlayer.enumMoves.tech; }
                    }
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
                else if (Input.GetKeyDown(controls.jump) && pChar.CanJump()) { qInput = sPlayer.enumMoves.jump; }
            }
        }

        //Start frame buffer on a new input
        if (qInput != qTemp)
        {
            xBuf = controls.buffer;
            //Debug.Log("Input received: " + qInput + "   Fixed Update #" + debugFramesFixed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        debugFrames++;

        //Action processing
        if (pChar.IsActable())
        {
            //Queued input from buffer
            if (qInput != sPlayer.enumMoves.none)
            {
                //Debug.Log("Action pulled from buffer: " + qInput + "   Update #" + debugFrames);

                ////////////////////
                //Possible actions//
                ////////////////////
                if (!pChar.IsAirborne())
                {
                    //Bufferable grounded move processing
                    if (qInput == sPlayer.enumMoves.jump) { pChar.GetCharAnimator.Play("JumpSquat"); }
                    else if (qInput == sPlayer.enumMoves.jab) { pChar.GetCharAnimator.Play("Jab"); }
                    else if (qInput == sPlayer.enumMoves.fLight) { pChar.GetCharAnimator.Play("ForwardLight"); }
                    else if (qInput == sPlayer.enumMoves.uLight) { pChar.GetCharAnimator.Play("UpLight"); }
                    else if (qInput == sPlayer.enumMoves.dLight) { pChar.GetCharAnimator.Play("DownLight"); }
                    else if (qInput == sPlayer.enumMoves.fStrong) { pChar.GetCharAnimator.Play("ForwardStrong"); }
                    else if (qInput == sPlayer.enumMoves.uStrong) { pChar.GetCharAnimator.Play("UpStrong"); }
                    else if (qInput == sPlayer.enumMoves.dStrong) { pChar.GetCharAnimator.Play("DownStrong"); }
                    else if (qInput == sPlayer.enumMoves.nSpec) { pChar.GetCharAnimator.Play("NeutralSpecial"); }
                    else if (qInput == sPlayer.enumMoves.fSpec) { pChar.GetCharAnimator.Play("ForwardSpecial"); }
                    else if (qInput == sPlayer.enumMoves.uSpec) { pChar.GetCharAnimator.Play("UpSpecial"); }
                    else if (qInput == sPlayer.enumMoves.dSpec) { pChar.GetCharAnimator.Play("DownSpecial"); }
                    else if (qInput == sPlayer.enumMoves.dodge) { pChar.GetCharAnimator.Play("SpotDodge"); }
                    else if (qInput == sPlayer.enumMoves.fRoll) { pChar.GetCharAnimator.Play("ForwardRoll"); }
                    else if (qInput == sPlayer.enumMoves.bRoll) { pChar.GetCharAnimator.Play("BackwardRoll"); }
                    else if (qInput == sPlayer.enumMoves.grab) { pChar.GetCharAnimator.Play("Grabbing"); }
                }
                else
                {
                    //Bufferable aerial move processing
                    if (qInput == sPlayer.enumMoves.jump) { pChar.GetCharAnimator.Play("AirJump"); pChar.Jump('a'); }
                    else if (qInput == sPlayer.enumMoves.nAir) { pChar.GetCharAnimator.Play("NeutralAir"); }
                    else if (qInput == sPlayer.enumMoves.fAir) { pChar.GetCharAnimator.Play("ForwardAir"); }
                    else if (qInput == sPlayer.enumMoves.bAir) { pChar.GetCharAnimator.Play("BackAir"); }
                    else if (qInput == sPlayer.enumMoves.uAir) { pChar.GetCharAnimator.Play("UpAir"); }
                    else if (qInput == sPlayer.enumMoves.dAir) { pChar.GetCharAnimator.Play("DownAir"); }
                    else if (qInput == sPlayer.enumMoves.nSpec) { pChar.GetCharAnimator.Play("NeutralSpecial"); }
                    else if (qInput == sPlayer.enumMoves.fSpec) { pChar.GetCharAnimator.Play("ForwardSpecial"); }
                    else if (qInput == sPlayer.enumMoves.uSpec) { pChar.GetCharAnimator.Play("UpSpecial"); }
                    else if (qInput == sPlayer.enumMoves.dSpec) { pChar.GetCharAnimator.Play("DownSpecial"); }
                    else if (qInput == sPlayer.enumMoves.airdodge
                            || qInput == sPlayer.enumMoves.fRoll
                            || qInput == sPlayer.enumMoves.bRoll
                            || qInput == sPlayer.enumMoves.tech
                            || qInput == sPlayer.enumMoves.techHop) { pChar.GetCharAnimator.Play("AirDodge"); }
                }

                //Buffered action executed
                qInput = sPlayer.enumMoves.none;
                xBuf = 0;
            }
            //Process non-bufferable actions (like movement)
            else
            {
                //Grounded actions
                if (!pChar.IsAirborne())
                {
                    //Grab actions
                    if (pChar.IsHoldingPlayer())
                    {

                    }
                    //Enter Block
                    if (Input.GetKeyDown(controls.block) && !pChar.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Blocking")) { pChar.GetCharAnimator.Play("Blocking"); }
                    //Key movement
                    else if (Input.GetKey(controls.down))
                    {
                        if (!pChar.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Crouching")) { pChar.GetCharAnimator.Play("Crouching"); }
                    }
                    else if (Input.GetKey(controls.right))
                    {
                        if (pChar.orientation != 1 || !pChar.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
                        {
                            pChar.orientation = 1;
                            pChar.GetCharAnimator.Play("Running");
                        }
                    }
                    else if (Input.GetKey(controls.left))
                    {
                        if (pChar.orientation != -1 || !pChar.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
                        {
                            pChar.orientation = -1;
                            pChar.GetCharAnimator.Play("Running");
                        }
                    }
                    //Analog Movement
                    else if (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > Mathf.Abs(Input.GetAxis(controls.moveVert)))
                    {
                        pChar.GetCharAnimator.Play("Running");
                    }
                    else if (Input.GetAxis(controls.moveVert) < -.5)
                    { 
                        //Crouch
                        pChar.GetCharAnimator.Play("Crouching");
                    }
                    else
                    {
                        pChar.GetCharAnimator.Play("Idle");
                    }
                }
                else if (!pChar.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Airborne")) //Aerial control
                {
                    pChar.GetCharAnimator.Play("Airborne");
                    //Check for fastfall
                    if (!pChar.IsFastfall() && Input.GetAxis(controls.moveVert) < .9) { pChar.ModFastfall(); }
                }
            }
        }
        //Process influence to inactable states (Teching, DI, mashing, ext.)
        else
        {
            //Process DI
            if (pChar.IsAirborne())
            {

            }

            //Process teching
            if (pChar.OnStage() && pChar.IsAirborne() && (qInput == sPlayer.enumMoves.airdodge
                            || qInput == sPlayer.enumMoves.fRoll
                            || qInput == sPlayer.enumMoves.bRoll
                            || qInput == sPlayer.enumMoves.tech
                            || qInput == sPlayer.enumMoves.techHop))
            {
                if (qInput == sPlayer.enumMoves.fRoll) { pChar.GetCharAnimator.Play("ForwardRoll"); }
                else if (qInput == sPlayer.enumMoves.bRoll) { pChar.GetCharAnimator.Play("BackwardRoll"); }
                else if (qInput == sPlayer.enumMoves.techHop) { pChar.GetCharAnimator.Play("TechHop"); }
                else { pChar.GetCharAnimator.Play("TechInPlace"); }

                pChar.ModAirborne();
                pChar.ModActable();
                qInput = sPlayer.enumMoves.none;
                xBuf = 0;
            }
            else if (pChar.OnWall() && (qInput == sPlayer.enumMoves.airdodge
                            || qInput == sPlayer.enumMoves.fRoll
                            || qInput == sPlayer.enumMoves.bRoll
                            || qInput == sPlayer.enumMoves.tech
                            || qInput == sPlayer.enumMoves.techHop))
            {
                pChar.GetCharAnimator.Play("WallJump");

                pChar.ModActable();
                qInput = sPlayer.enumMoves.none;
                xBuf = 0;
            }
            else if (!pChar.IsAirborne() && !pChar.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("KnockedDown"))
            {
                pChar.GetCharAnimator.Play("KnockedDown");
            }

            //Process actions out of knockdown
            if (pChar.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("KnockedDown"))
            {
                if (qInput == sPlayer.enumMoves.fRoll) { pChar.GetCharAnimator.Play("ForwardRoll"); }
                else if (qInput == sPlayer.enumMoves.bRoll) { pChar.GetCharAnimator.Play("BackwardRoll"); }
                else if (qInput == sPlayer.enumMoves.getup) { pChar.GetCharAnimator.Play("Getup"); }
                else if (qInput == sPlayer.enumMoves.getupAtk) { pChar.GetCharAnimator.Play("GetupAttack"); }
            }
        }

        //Debuging using moves after actable states
        /*if (qInput == sPlayer.enumMoves.jab && (pChar.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || pChar.GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Running")))
        {
            Debug.Log("Should be able to jab");
            if(pChar.isActable()) { Debug.Log( "Actable"); }
            else { Debug.Log("Error: Not Actable when should be actable"); }
        }
        else if(Input.GetKeyUp(controls.right) || Input.GetKeyUp(controls.left))
        {
            Debug.Log("Stopped moving with no jab in buffer");
            if (actable) { Debug.Log("Actable"); }
            else { Debug.Log("Error: Not Actable when should be actable"); }
        }*/
    }

    public ControlScheme GetControls { get { return controls; } }
    public void SetControls(ControlScheme ctrls)
    {
        controls = ctrls;
    }
    public void SetControls(int bfr, string mH, string mV, InputAction rSU, string rH, string rV, KeyCode l, KeyCode r, KeyCode u, KeyCode d,
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
    public void SetControls(TextAsset prf)
    {
        StreamReader reader = new StreamReader("Assets/Text/" + prf.name + ".txt");

        controls.buffer = int.Parse(reader.ReadLine().Split(new char[] {' '}, 2, System.StringSplitOptions.None)[1]);
        controls.moveHorz = reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1];
        controls.moveVert = reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1];
        controls.rStickUse = (InputAction)System.Enum.Parse( typeof(InputAction), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
        controls.rHorz = reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1];
        controls.rVert = reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1];
        controls.left = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
        controls.right = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
        controls.up = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
        controls.down = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
        controls.light = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
        controls.heavy = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
        controls.lightToHeavy = float.Parse(reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
        controls.special = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
        controls.block = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
        controls.grab = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
        controls.jump = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
        controls.alt = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
        controls.pause = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
    }
}

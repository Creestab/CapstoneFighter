using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sInput : MonoBehaviour
{ 
    sPlayer pChar;
    sData.ControlScheme controls;

    sData.MoveType qInput;
    int xBuf;
    bool forceHeavy;

    // Start is called before the first frame update
    void Start()
    {
        qInput = sData.MoveType.none;
        xBuf = 0;
        forceHeavy = false;

        Refresh();
        ResetBuffer();
    }

    private void Update()
    {
        //Debug analog sticks
        //if (Input.GetKeyDown(controls.alt)) { Debug.Log("Printing axis values..."); }
        if (Input.GetKeyDown(controls.alt))
        {
            Debug.Log("   Left Analog Horizontal: " + Input.GetAxis("P1_LHorz"));
            //Debug.Log("   Left Analog Vertical: " + Input.GetAxis("P1_LVert"));
            //Debug.Log("   Right Analog Horizontal: " + Input.GetAxis("P1_RHorz"));
            //Debug.Log("   Right Analog Vertical: " + Input.GetAxis("P1_RVert"));
            //Debug.Log("   Left Analog Horizontal: " + Input.GetAxis("P2_LHorz"));
            //Debug.Log("   Left Analog Vertical: " + Input.GetAxis("P2_LVert"));
            //Debug.Log("   Right Analog Horizontal: " + Input.GetAxis("P2_RHorz"));
            //Debug.Log("   Right Analog Vertical: " + Input.GetAxis("P2_RVert"));
        }

        //Clear buffered actions
        qInput = sData.MoveType.none;
        if(xBuf > 0) xBuf--;
        
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
                if (Input.GetKeyDown(controls.light) || (controls.rStickUse == sData.InputAction.Light && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0)))
                {
                    //Is it a shield grab?
                    if (Input.GetKey(controls.block))
                    {
                        qInput = sData.MoveType.grab;
                    }
                    //Handling of right stick input
                    else if (controls.rStickUse == sData.InputAction.Light && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0))
                    {
                        //What type of light?
                        if (Mathf.Abs(Input.GetAxis(controls.rHorz)) > Mathf.Abs(Input.GetAxis(controls.rVert)))
                        {
                            //Forward light
                            if (Input.GetAxis(controls.rHorz) > 0) { qInput = sData.MoveType.fLight; }
                            //Pivot Forward light
                            else
                            {
                                pChar.orientation = -pChar.orientation;
                                qInput = sData.MoveType.fLight;
                            }
                        }
                        else
                        {
                            //Up light
                            if (Input.GetAxis(controls.rVert) > 0) { qInput = sData.MoveType.uLight; }
                            //Down light
                            else { qInput = sData.MoveType.dLight; }
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
                                if (Input.GetAxis(controls.moveHorz) > 0) { qInput = sData.MoveType.fLight; }
                                //Pivot Forward light
                                else
                                {
                                    pChar.orientation = -pChar.orientation;
                                    qInput = sData.MoveType.fLight;
                                }
                            }
                            else
                            {
                                //Up light
                                if (Input.GetAxis(controls.moveVert) > 0) { qInput = sData.MoveType.uLight; }
                                //Down light
                                else { qInput = sData.MoveType.dLight; }
                            }
                        }
                        else//Its a jab
                        {
                            qInput = sData.MoveType.jab;
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
                    if (Input.GetAxis(controls.moveHorz) > .5) { qInput = sData.MoveType.fRoll; }
                    //Roll left
                    else if (Input.GetAxis(controls.moveHorz) < -.5) { qInput = sData.MoveType.bRoll; }
                    else if (Input.GetAxis(controls.moveVert) < -.5) { qInput = sData.MoveType.dodge; }
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
                            if (Input.GetAxis(controls.moveHorz) > 0) { qInput = sData.MoveType.fSpec; }
                            //Pivot Forward special
                            else
                            {
                                pChar.orientation = -pChar.orientation;
                                qInput = sData.MoveType.fSpec;
                            }
                        }
                        else
                        {
                            //Up special
                            if (Input.GetAxis(controls.moveVert) > 0)
                            {
                                if (Input.GetAxis(controls.moveHorz) < -.1) { pChar.orientation = -pChar.orientation; }
                                qInput = sData.MoveType.uSpec;
                            }
                            //Down special
                            else { qInput = sData.MoveType.dSpec; }
                        }
                    }
                    else //Its a neutral special
                    {
                        qInput = sData.MoveType.nSpec;
                    }
                }
                else if (forceHeavy || Input.GetKeyDown(controls.heavy) || (controls.rStickUse == sData.InputAction.Heavy && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0)))
                {
                    //Is it non-directional?
                    if ((Input.GetKeyDown(controls.heavy) || forceHeavy) && (Input.GetAxis(controls.moveHorz) != 0 || Input.GetAxis(controls.moveVert) != 0))
                    {
                        //What type of strong?
                        if (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > Mathf.Abs(Input.GetAxis(controls.moveVert)))
                        {
                            //Forward strong
                            if (Input.GetAxis(controls.moveHorz) > 0) { qInput = sData.MoveType.fStrong; }
                            //Pivot Forward strong
                            else
                            {
                                pChar.orientation = -pChar.orientation;
                                qInput = sData.MoveType.fStrong;
                            }
                        }
                        else
                        {
                            //Up strong
                            if (Input.GetAxis(controls.moveVert) > 0) { qInput = sData.MoveType.uStrong; }
                            //Down strong
                            else { qInput = sData.MoveType.dStrong; }
                        }
                    }
                    //Handling of c-stick input
                    else if (controls.rStickUse == sData.InputAction.Heavy && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0))
                    {
                        //What type of strong?
                        if (Mathf.Abs(Input.GetAxis(controls.rHorz)) > Mathf.Abs(Input.GetAxis(controls.rVert)))
                        {
                            //Forward strong
                            if (Input.GetAxis(controls.rHorz) > 0) { qInput = sData.MoveType.fStrong; }
                            //Pivot Forward strong
                            else
                            {
                                pChar.orientation = -pChar.orientation;
                                qInput = sData.MoveType.fStrong;
                            }
                        }
                        else
                        {
                            //Up strong
                            if (Input.GetAxis(controls.rVert) > 0) { qInput = sData.MoveType.uStrong; }
                            //Down strong
                            else { qInput = sData.MoveType.dStrong; }
                        }
                    }
                    else //Dedicated strong button with no direction, default to forward strong
                    {
                        qInput = sData.MoveType.fStrong;
                    }
                }
                else if (Input.GetKeyDown(controls.grab)) { qInput = sData.MoveType.grab; }
                else if (Input.GetKeyDown(controls.jump)) { qInput = sData.MoveType.jump; }
            }
            //The player is airborne
            else
            {
                //Is the player inputting an attack?
                if (Input.GetKeyDown(controls.light) || Input.GetKeyDown(controls.heavy) ||
                    ((controls.rStickUse == sData.InputAction.Light || controls.rStickUse == sData.InputAction.Heavy) && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0)))
                {
                    //Handling of right stick input
                    if ((controls.rStickUse == sData.InputAction.Light || controls.rStickUse == sData.InputAction.Heavy) && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0))
                    {
                        //What type of aerial?
                        if (Mathf.Abs(Input.GetAxis(controls.rHorz)) > Mathf.Abs(Input.GetAxis(controls.rVert)))
                        {
                            //Forward air
                            if (Input.GetAxis(controls.rHorz) > 0) { qInput = sData.MoveType.fAir; }
                            //Back air
                            else { qInput = sData.MoveType.bAir; }
                        }
                        else
                        {
                            //Up air
                            if (Input.GetAxis(controls.rVert) > 0) { qInput = sData.MoveType.uAir; }
                            //Down air
                            else { qInput = sData.MoveType.dAir; }
                        }
                    }
                    //Is it not a neautral air?
                    else if ((Input.GetKeyDown(controls.light) || Input.GetKeyDown(controls.heavy)) && (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > .25 || Mathf.Abs(Input.GetAxis(controls.moveVert)) > .25))
                    {
                        //What type of aerial?
                        if (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > Mathf.Abs(Input.GetAxis(controls.moveVert)))
                        {
                            //Forward air
                            if (Input.GetAxis(controls.moveHorz) > 0) { qInput = sData.MoveType.fAir; }
                            //back air
                            else { qInput = sData.MoveType.bAir; }
                        }
                        else
                        {
                            //Up air
                            if (Input.GetAxis(controls.moveVert) > 0) { qInput = sData.MoveType.uAir; }
                            //Down light
                            else { qInput = sData.MoveType.dAir; }
                        }
                    }
                    else if (Input.GetKeyDown(controls.light) || Input.GetKeyDown(controls.heavy))//Its it a nair
                    {
                        qInput = sData.MoveType.nAir;
                    }
                }
                //Is the player inputting airdoge/tech?
                else if (Input.GetKeyDown(controls.block) || Input.GetKeyDown(controls.grab))
                {
                    //What type of tech?
                    if (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > Mathf.Abs(Input.GetAxis(controls.moveVert)))
                    {
                        //Tech Forward Roll
                        if (Input.GetAxis(controls.moveHorz) > 0) { qInput = sData.MoveType.fRoll; }
                        //Tech Backward Roll
                        else { qInput = sData.MoveType.bRoll; }
                    }
                    else
                    {
                        //Tech hop
                        if (Input.GetAxis(controls.moveVert) > 0) { qInput = sData.MoveType.techHop; }
                        //Default to normal tech
                        else { qInput = sData.MoveType.tech; }
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
                            if (Input.GetAxis(controls.moveHorz) > 0) { qInput = sData.MoveType.fSpec; }
                            //Pivot Forward special
                            else
                            {
                                pChar.orientation = -pChar.orientation;
                                qInput = sData.MoveType.fSpec;
                            }
                        }
                        else
                        {
                            //Up special
                            if (Input.GetAxis(controls.moveVert) > 0)
                            {
                                if (Input.GetAxis(controls.moveHorz) < -.1) { pChar.orientation = -pChar.orientation; }
                                qInput = sData.MoveType.uSpec;
                            }
                            //Down special
                            else { qInput = sData.MoveType.dSpec; }
                        }
                    }
                    else //Its a neutral special
                    {
                        qInput = sData.MoveType.nSpec;
                    }
                }
                else if (Input.GetKeyDown(controls.jump) && pChar.CanJump()) { qInput = sData.MoveType.jump; }
            }
        }
        //The player is facing left
        else
        {
            //Is the player on the ground?
            if (!pChar.IsAirborne())
            {
                //Is the player inputting a light attack?
                if (Input.GetKeyDown(controls.light) || (controls.rStickUse == sData.InputAction.Light && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0)))
                {
                    //Is it a shield grab?
                    if (Input.GetKey(controls.block))
                    {
                        qInput = sData.MoveType.grab;
                    }
                    //Handling of right stick input
                    else if (controls.rStickUse == sData.InputAction.Light && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0))
                    {
                        //What type of light?
                        if (Mathf.Abs(Input.GetAxis(controls.rHorz)) > Mathf.Abs(Input.GetAxis(controls.rVert)))
                        {
                            //Forward light
                            if (Input.GetAxis(controls.rHorz) < 0) { qInput = sData.MoveType.fLight; }
                            //Pivot Forward light
                            else
                            {
                                pChar.orientation = -pChar.orientation;
                                qInput = sData.MoveType.fLight;
                            }
                        }
                        else
                        {
                            //Up light
                            if (Input.GetAxis(controls.rVert) > 0) { qInput = sData.MoveType.uLight; }
                            //Down light
                            else { qInput = sData.MoveType.dLight; }
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
                                if (Input.GetAxis(controls.moveHorz) < 0) { qInput = sData.MoveType.fLight; }
                                //Pivot Forward light
                                else
                                {
                                    pChar.orientation = -pChar.orientation;
                                    qInput = sData.MoveType.fLight;
                                }
                            }
                            else
                            {
                                //Up light
                                if (Input.GetAxis(controls.moveVert) > 0) { qInput = sData.MoveType.uLight; }
                                //Down light
                                else { qInput = sData.MoveType.dLight; }
                            }
                        }
                        else //Its a jab
                        {
                            qInput = sData.MoveType.jab;
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
                    if (Input.GetAxis(controls.moveHorz) < -.5) { qInput = sData.MoveType.fRoll; }
                    //Roll left
                    else if (Input.GetAxis(controls.moveHorz) > .5) { qInput = sData.MoveType.bRoll; }
                    else if (Input.GetAxis(controls.moveVert) < -.5) { qInput = sData.MoveType.dodge; }
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
                            if (Input.GetAxis(controls.moveHorz) < 0) { qInput = sData.MoveType.fSpec; }
                            //Pivot Forward special
                            else
                            {
                                pChar.orientation = -pChar.orientation;
                                qInput = sData.MoveType.fSpec;
                            }
                        }
                        else
                        {
                            //Up special
                            if (Input.GetAxis(controls.moveVert) > 0)
                            {
                                if (Input.GetAxis(controls.moveHorz) > .1) { pChar.orientation = -pChar.orientation; }
                                qInput = sData.MoveType.uSpec;
                            }
                            //Down special
                            else { qInput = sData.MoveType.dSpec; }
                        }
                    }
                    else //Its a neutral special
                    {
                        qInput = sData.MoveType.nSpec;
                    }
                }
                //Is the player inputting a heavy attack?
                else if (forceHeavy || Input.GetKeyDown(controls.heavy) || (controls.rStickUse == sData.InputAction.Heavy && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0)))
                {
                    //Is it non-directional?
                    if ((Input.GetKeyDown(controls.heavy) || forceHeavy) && (Input.GetAxis(controls.moveHorz) != 0 || Input.GetAxis(controls.moveVert) != 0))
                    {
                        //What type of strong?
                        if (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > Mathf.Abs(Input.GetAxis(controls.moveVert)))
                        {
                            //Forward strong
                            if (Input.GetAxis(controls.moveHorz) < 0) { qInput = sData.MoveType.fStrong; }
                            //Pivot Forward strong
                            else
                            {
                                pChar.orientation = -pChar.orientation;
                                qInput = sData.MoveType.fStrong;
                            }
                        }
                        else
                        {
                            //Up strong
                            if (Input.GetAxis(controls.moveVert) > 0) { qInput = sData.MoveType.uStrong; }
                            //Down strong
                            else { qInput = sData.MoveType.dStrong; }
                        }
                    }
                    //Handling of right stick input
                    else if (controls.rStickUse == sData.InputAction.Heavy && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0))
                    {
                        //What type of strong?
                        if (Mathf.Abs(Input.GetAxis(controls.rHorz)) > Mathf.Abs(Input.GetAxis(controls.rVert)))
                        {
                            //Forward strong
                            if (Input.GetAxis(controls.rHorz) < 0) { qInput = sData.MoveType.fStrong; }
                            //Pivot Forward strong
                            else
                            {
                                pChar.orientation = -pChar.orientation;
                                qInput = sData.MoveType.fStrong;
                            }
                        }
                        else
                        {
                            //Up strong
                            if (Input.GetAxis(controls.rVert) > 0) { qInput = sData.MoveType.uStrong; }
                            //Down strong
                            else { qInput = sData.MoveType.dStrong; }
                        }
                    }
                    else //Dedicated strong button with no direction, default to forward strong
                    {
                        qInput = sData.MoveType.fStrong;
                    }
                }
                else if (Input.GetKeyDown(controls.grab)) { qInput = sData.MoveType.grab; }
                else if (Input.GetKeyDown(controls.jump)) { qInput = sData.MoveType.jump; }
            }
            //The player is airborne
            else
            {
                //Is the player inputting an attack?
                if (Input.GetKeyDown(controls.light) || Input.GetKeyDown(controls.heavy) ||
                    ((controls.rStickUse == sData.InputAction.Light || controls.rStickUse == sData.InputAction.Heavy) && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0)))
                {
                    //Handling of right stick input
                    if ((controls.rStickUse == sData.InputAction.Light || controls.rStickUse == sData.InputAction.Heavy) && (Input.GetAxis(controls.rHorz) != 0 || Input.GetAxis(controls.rVert) != 0))
                    {
                        //What type of aerial?
                        if (Mathf.Abs(Input.GetAxis(controls.rHorz)) > Mathf.Abs(Input.GetAxis(controls.rVert)))
                        {
                            //Forward air
                            if (Input.GetAxis(controls.rHorz) < 0) { qInput = sData.MoveType.fAir; }
                            //Back air
                            else { qInput = sData.MoveType.bAir; }
                        }
                        else
                        {
                            //Up air
                            if (Input.GetAxis(controls.rVert) > 0) { qInput = sData.MoveType.uAir; }
                            //Down air
                            else { qInput = sData.MoveType.dAir; }
                        }
                    }
                    //Is it not a neautral air?
                    else if ((Input.GetKeyDown(controls.light) || Input.GetKeyDown(controls.heavy)) && (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > .25 || Mathf.Abs(Input.GetAxis(controls.moveVert)) > .25))
                    {
                        //What type of aerial?
                        if (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > Mathf.Abs(Input.GetAxis(controls.moveVert)))
                        {
                            //Forward air
                            if (Input.GetAxis(controls.moveHorz) < 0) { qInput = sData.MoveType.fAir; }
                            //back air
                            else { qInput = sData.MoveType.bAir; }
                        }
                        else
                        {
                            //Up air
                            if (Input.GetAxis(controls.moveVert) > 0) { qInput = sData.MoveType.uAir; }
                            //Down light
                            else { qInput = sData.MoveType.dAir; }
                        }
                    }
                    else if (Input.GetKeyDown(controls.light) || Input.GetKeyDown(controls.heavy))//Its it a nair
                    {
                        qInput = sData.MoveType.nAir;
                    }
                }
                //Is the player inputting airdoge/tech?
                else if (Input.GetKeyDown(controls.block) || Input.GetKeyDown(controls.grab))
                {
                    //What type of tech?
                    if (Mathf.Abs(Input.GetAxis(controls.moveHorz)) > Mathf.Abs(Input.GetAxis(controls.moveVert)))
                    {
                        //Tech Back Roll
                        if (Input.GetAxis(controls.moveHorz) > 0) { qInput = sData.MoveType.bRoll; }
                        //Tech Forward Roll
                        else { qInput = sData.MoveType.fRoll; }
                    }
                    else
                    {
                        //Tech hop
                        if (Input.GetAxis(controls.moveVert) > 0) { qInput = sData.MoveType.techHop; }
                        //Default to normal tech
                        else { qInput = sData.MoveType.tech; }
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
                            if (Input.GetAxis(controls.moveHorz) < 0) { qInput = sData.MoveType.fSpec; }
                            //Pivot Forward special
                            else
                            {
                                pChar.orientation = -pChar.orientation;
                                qInput = sData.MoveType.fSpec;
                            }
                        }
                        else
                        {
                            //Up special
                            if (Input.GetAxis(controls.moveVert) > 0)
                            {
                                if (Input.GetAxis(controls.moveHorz) > .1) { pChar.orientation = -pChar.orientation; }
                                qInput = sData.MoveType.uSpec;
                            }
                            //Down special
                            else { qInput = sData.MoveType.dSpec; }
                        }
                    }
                    else //Its a neutral special
                    {
                        qInput = sData.MoveType.nSpec;
                    }
                }
                else if (Input.GetKeyDown(controls.jump) && pChar.CanJump()) { qInput = sData.MoveType.jump; }
            }
        }

        //Start frame buffer on a new input
        if (qInput != sData.MoveType.none)
        {
            xBuf = controls.buffer;
            pChar.SendInput(qInput);
            //Debug.Log("Input received: " + qInput + "   Fixed Update #" + debugFramesFixed);
        }
        else if (!pChar.NoInput() && xBuf <= 0) pChar.ClearInput();
    }

    public void ResetBuffer()
    {
        qInput = sData.MoveType.none;
        xBuf = 0;
    }

    public void Refresh()
    {
        pChar = gameObject.GetComponent<sPlayer>();
        SetControls(gameObject.GetComponent<sPlayer>().ctrlProfile);
    }

    public sData.ControlScheme GetControls()
    {
        return controls;
    }
    public void SetControls()
    {
        controls = sData.ReadControls(gameObject.GetComponent<sPlayer>().ctrlProfile);
    }
    public void SetControls(string name)
    {
        controls = sData.ReadControls(name);
    }
    public void SetControls (sData.ControlScheme ctrls)
    {
        controls = ctrls;
    }
    
    override public string ToString()
    {
        string s=
            "Associated Player: " + pChar.pNumber + '\n' +
            "Player Verification: ";
                if(pChar.GetHashCode() == gameObject.GetComponent<sPlayer>().GetHashCode()) { s+= "OK" + '\n'; } else { s+= "ERROR" + '\n'; }
        s+= "Control Scheme file of Player: " + pChar.ctrlProfile + '\n' +
            "File Verification: ";
                if(pChar.ctrlProfile == gameObject.GetComponent<sPlayer>().ctrlProfile) { s+= "OK" + '\n'; } else { s+= "ERROR" + '\n'; }
        s+= '\n' + "~Player Controls~" + '\n' + controls.ToString() + '\n' +
            "Current Buffered Input: " + qInput + '\n' +
            "Frames of Buffer Remaining: " + xBuf + '\n';

        return s;
    }
}

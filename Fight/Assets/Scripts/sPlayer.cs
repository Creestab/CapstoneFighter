﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSensorData
{
    [SerializeField] sSensor[] _sensors;
    [SerializeField] sData.MoveType _atkType;

    public sSensor[] GetSensors { get { return _sensors; } }
    public sData.MoveType GetAtkType { get { return _atkType; } }
}

public class sPlayer : MonoBehaviour
{
    [SerializeField] Animator _anim;
    [SerializeField] List<PlayerSensorData> _sensors;

    public Dictionary<sData.MoveType, float[,]> moves;
    public string ctrlProfile;
    private GameObject rig;
    private Rigidbody rb;
    private BoxCollider box;

    //Characters state
    public int orientation; //1 for facing right, -1 for facing left.
    public int maxJumps;
    int jumps;

    float dmg;
    int stun;
    bool actable;
    bool onStage;
    bool onWall;
    bool airborne;
    bool fastfall;
    bool invincible;
    bool holdingPlayer;

    //Character traits
    public int pNumber;
    public Color pColor;
    public float moveSpeed;
    public float jumpSpeed;
    public float airjumpSpeed;
    public float fallSpeed;
    public float maxFallSpeed;

    //State modifications and scaling
    private static float zStart;
    private static float gMoveMod = 5;   //Scales the speed of grounded movement
    private static float aMoveMod = 4;    //Scales the speed of aerial movement
    private static float jHeightMod = 50;  //Scales the force of jumping
    private static float ffMod = 2;       //Scales the force of fastfalling

    //Current momentum
    private float mVert;     //Vertical force

    private sData.MoveType input;

    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    private void Update()
    {
        if(gameObject.transform.position.z != zStart) gameObject.transform.position.Set(gameObject.transform.position.x, gameObject.transform.position.y, zStart);
        if (rb.velocity.z != 0) rb.velocity.Set(rb.velocity.x, rb.velocity.y, 0);

        //Reset animation speed when not running
        if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Running")) { _anim.speed = 1; }
        //Process current state
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || _anim.GetCurrentAnimatorStateInfo(0).IsName("Running")
                || _anim.GetCurrentAnimatorStateInfo(0).IsName("Crouching") || _anim.GetCurrentAnimatorStateInfo(0).IsName("Blocking"))
        {
            //Drops shield when not holding the input anymore
            if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Blocking") && Input.GetKeyUp(gameObject.GetComponent<sInput>().GetControls().block)) { _anim.Play("Idle"); }

            actable = true;
        }
        else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Airborne") || _anim.GetCurrentAnimatorStateInfo(0).IsName("AirJump"))
        {
            actable = true;
        }
        else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("JumpSquat"))
        {
            actable = false;
            if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= (5/6))
            {
                Debug.Log("End of JumpSquat");
                actable = true;
                if (Input.GetKey(gameObject.GetComponent<sInput>().GetControls().jump)) Jump('f');
                else Jump('s');

                if (input == sData.MoveType.jump) input = sData.MoveType.none;
            }
        }
        else
        {
            actable = false;
        }

        mVert = 0;

        //Action processing
        if (IsActable())
        {
            //Queued input from buffer
            if (input != sData.MoveType.none)
            {
                //Debug.Log("Action pulled from buffer: " + input + "   Update #" + debugFrames);

                ////////////////////
                //Possible actions//
                ////////////////////

                if(airborne)
                {
                    //Bufferable aerial move processing
                    if (input == sData.MoveType.jump) { Jump('a'); }
                    else if (input == sData.MoveType.nAir) { _anim.Play("NeutralAir"); }
                    //else if (input == sData.MoveType.fAir) { _anim.Play("ForwardAir"); }
                    //else if (input == sData.MoveType.bAir) { _anim.Play("BackAir"); }
                    //else if (input == sData.MoveType.uAir) { _anim.Play("UpAir"); }
                    //else if (input == sData.MoveType.dAir) { _anim.Play("DownAir"); }
                    else if (input == sData.MoveType.nSpec) { _anim.Play("NeutralSpecial"); }
                    //else if (input == sData.MoveType.fSpec) { _anim.Play("ForwardSpecial"); }
                    //else if (input == sData.MoveType.uSpec) { _anim.Play("UpSpecial"); }
                    //else if (input == sData.MoveType.dSpec) { _anim.Play("DownSpecial"); }
                    else if (input == sData.MoveType.airdodge
                            || input == sData.MoveType.fRoll
                            || input == sData.MoveType.bRoll
                            || input == sData.MoveType.tech
                            || input == sData.MoveType.techHop) { _anim.Play("AirDodge"); }
                }
                else
                {
                    //Bufferable grounded move processing
                    if (input == sData.MoveType.jump) { _anim.Play("JumpSquat"); Debug.Log("Preparing for jump"); }
                    else if (input == sData.MoveType.jab) { _anim.Play("Jab"); }
                    //else if (input == sData.MoveType.fLight) { _anim.Play("ForwardLight"); }
                    //else if (input == sData.MoveType.uLight) { _anim.Play("UpLight"); }
                    //else if (input == sData.MoveType.dLight) { _anim.Play("DownLight"); }
                    //else if (input == sData.MoveType.fStrong) { _anim.Play("ForwardStrong"); }
                    //else if (input == sData.MoveType.uStrong) { _anim.Play("UpStrong"); }
                    //else if (input == sData.MoveType.dStrong) { _anim.Play("DownStrong"); }
                    //else if (input == sData.MoveType.nSpec) { _anim.Play("NeutralSpecial"); }
                    //else if (input == sData.MoveType.fSpec) { _anim.Play("ForwardSpecial"); }
                    else if (input == sData.MoveType.uSpec) { _anim.Play("UpSpecial"); }
                    //else if (input == sData.MoveType.dSpec) { _anim.Play("DownSpecial"); }
                    else if (input == sData.MoveType.dodge) { _anim.Play("SpotDodge"); }
                    //else if (input == sData.MoveType.fRoll) { _anim.Play("ForwardRoll"); }
                    //else if (input == sData.MoveType.bRoll) { _anim.Play("BackwardRoll"); }
                    //else if (input == sData.MoveType.grab) { _anim.Play("Grabbing"); }
                }

                //Buffered action executed
                gameObject.GetComponent<sInput>().ResetBuffer();
                ClearInput();
            }
            //Process non-bufferable actions (like movement)
            else
            {
                if (airborne) //Aerial control
                {
                    if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Airborne") && !_anim.GetCurrentAnimatorStateInfo(0).IsName("AirJump")) _anim.Play("Airborne");

                    if (!fastfall)
                    {
                        if ((Input.GetKeyDown(gameObject.GetComponent<sInput>().GetControls().down) || Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveVert) < -.7) && rb.velocity.y < 0 && rb.velocity.y > -fallSpeed * ffMod)
                        {
                            fastfall = true;
                            mVert = -fallSpeed * ffMod;
                            rb.velocity.Set(rb.velocity.x, -fallSpeed, 0);
                        }
                        else mVert = -fallSpeed;
                    }
                    else mVert = -fallSpeed * ffMod;

                    //Key Movement
                    if (Input.GetKey(gameObject.GetComponent<sInput>().GetControls().right))
                    {
                        gameObject.transform.position -= gameObject.transform.right * orientation * moveSpeed * (aMoveMod / 1000);
                    }
                    else if (Input.GetKey(gameObject.GetComponent<sInput>().GetControls().left))
                    {
                        gameObject.transform.position += gameObject.transform.right * orientation * moveSpeed * (aMoveMod / 1000);
                    }
                    else //Analog Movement
                    {
                        if (Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveHorz) > 0)
                        {
                            gameObject.transform.position -= gameObject.transform.right * orientation * moveSpeed * (aMoveMod / 1000) * Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveHorz);
                        }
                        else if (Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveHorz) < 0)
                        {
                            gameObject.transform.position -= gameObject.transform.right * orientation * moveSpeed * (aMoveMod / 1000) * Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveHorz);
                        }
                    }
                }
                else //Grounded actions
                {
                    mVert = -10;
                    if (rb.velocity.x != 0 || rb.velocity.y != 0) rb.velocity.Set(0, 0, 0);

                    //Grab actions
                    if (IsHoldingPlayer())
                    {

                    }
                    //Enter Block
                    else if (Input.GetKeyDown(gameObject.GetComponent<sInput>().GetControls().block) && !_anim.GetCurrentAnimatorStateInfo(0).IsName("Blocking")) { _anim.Play("Blocking"); }
                    //Key movement
                    else if (Input.GetKey(gameObject.GetComponent<sInput>().GetControls().down))
                    {
                        if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Crouching")) { _anim.Play("Crouching"); }
                    }
                    else if (Input.GetKey(gameObject.GetComponent<sInput>().GetControls().right))
                    {
                        if (orientation != 1) 
                        {
                            orientation = 1;
                        }
                        if(!_anim.GetCurrentAnimatorStateInfo(0).IsName("Running"))
                        { 
                            _anim.Play("Running");
                        }
                        gameObject.transform.position -= gameObject.transform.right * moveSpeed * (gMoveMod / 1000);
                    }
                    else if (Input.GetKey(gameObject.GetComponent<sInput>().GetControls().left))
                    {
                        if (orientation != -1)
                        {
                            orientation = -1;
                        }
                        if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Running"))
                        { 
                            _anim.Play("Running");
                        }
                        gameObject.transform.position -= gameObject.transform.right * moveSpeed * (gMoveMod / 1000);
                    }
                    //Analog Movement
                    else if (Mathf.Abs(Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveHorz)) > Mathf.Abs(Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveVert)))
                    {
                        if (Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveHorz) > 0) { orientation = 1; }
                        else { orientation = -1; }

                        if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Running")) _anim.Play("Running");
                        _anim.speed = Mathf.Abs(Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveHorz));

                        gameObject.transform.position -= gameObject.transform.right * moveSpeed * (gMoveMod / 1000) * Mathf.Abs(Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveHorz));
                    }
                    else if (Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveVert) < -.5)
                    {
                        //Crouch
                        if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Crouching")) _anim.Play("Crouching");
                    }
                    else
                    {
                        if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) _anim.Play("Idle");
                    }
                }
            }
        }
        //Process influence to inactable states (Performing a move, teching, DI, mashing, ext.)
        else
        {
            //Process teching
            if (airborne && stun > 0)
            {
                if (input == sData.MoveType.airdodge
                                || input == sData.MoveType.fRoll
                                || input == sData.MoveType.bRoll
                                || input == sData.MoveType.tech
                                || input == sData.MoveType.techHop)
                {
                    if (onStage)
                    {
                        if (input == sData.MoveType.fRoll) { _anim.Play("ForwardRoll"); }
                        else if (input == sData.MoveType.bRoll) { _anim.Play("BackwardRoll"); }
                        else if (input == sData.MoveType.techHop) { _anim.Play("TechHop"); }
                        else { _anim.Play("TechInPlace"); }

                        airborne = false;
                        gameObject.GetComponent<sInput>().ResetBuffer();
                    }
                    else if (onWall)
                    {
                        _anim.Play("WallJump");

                        gameObject.GetComponent<sInput>().ResetBuffer();
                    }
                }
                else if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("KnockedDown"))
                {
                    _anim.Play("KnockedDown");
                }
                else stun -= 5;
                if (stun < 0) stun = 0;
            }
            //Process actions out of knockdown
            else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("KnockedDown"))
            {
                if (input == sData.MoveType.fRoll) { _anim.Play("ForwardRoll"); }
                else if (input == sData.MoveType.bRoll) { _anim.Play("BackwardRoll"); }
                else if (input == sData.MoveType.getup) { _anim.Play("Getup"); }
                else if (input == sData.MoveType.getupAtk) { _anim.Play("GetupAttack"); }
            }
            //Process tumble control and DI
            else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Tumble"))
            {
                stun--;
                if (stun <= 0) { _anim.Play("Airborne"); stun = 0; }
            }
            else //Currently performing a move
            {
                //Aerial
                if(airborne)
                {

                }
                //Non-Aerial
                else
                {

                }
            }
        }

        //Debuging using moves after actable states
        /**if (input == sData.MoveType.jab && (_anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || _anim.GetCurrentAnimatorStateInfo(0).IsName("Running")))
        {
            Debug.Log("Should be able to jab");
            if(isActable()) { Debug.Log( "Actable"); }
            else { Debug.Log("Error: Not Actable when should be actable"); }
        }
        else if(Input.GetKeyUp(gameObject.GetComponent<sInput>().GetControls().right) || Input.GetKeyUp(controls.left))
        {
            Debug.Log("Stopped moving with no jab in buffer");
            if (actable) { Debug.Log("Actable"); }
            else { Debug.Log("Error: Not Actable when should be actable"); }
        }*/

        //Process orientation
        if (orientation == 1)
        {
            gameObject.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        else if (orientation == -1)
        {
            gameObject.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        
        rb.AddForce(new Vector3(0, mVert - (Mathf.Log10(Mathf.Abs(rb.velocity.y) + 100) * (fallSpeed - aMoveMod)), 0.0f));
        if (rb.velocity.y < -maxFallSpeed && stun == 0) { rb.velocity = new Vector3(rb.velocity.x, -maxFallSpeed, 0); }
    }

    public void Jump(char type)
    {
        Debug.Log("Attempting Jump");

        //Short jump
        if (type == 's')
        {
            rb.velocity = new Vector3(rb.velocity.x, 20.0f, 0.0f);
            mVert = jumpSpeed * jHeightMod / 2;
        }
        //Full jump
        else if (type == 'f')
        {
            rb.velocity = new Vector3(rb.velocity.x, 40.0f, 0.0f);
            mVert = jumpSpeed * jHeightMod;
        }
        //Aerial jump
        else if (type == 'a')
        {
            _anim.Play("AirJump");
            rb.velocity = new Vector3(rb.velocity.x, 10.0f, 0.0f);
            mVert = airjumpSpeed * jHeightMod;
            jumps--;
        }

        if (!airborne)
        {
            airborne = true;
            gameObject.transform.Translate(new Vector3(0, .5f, 0), Space.Self);
            _anim.Play("Airborne");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.material == null || collision.collider.material.name.Equals(""))
        {

        }
        else if (collision.collider.material.name.Contains("pmPlayer"))
        {
            if (!collision.transform.root.gameObject.Equals(gameObject))
            {
                sPlayer pOther = collision.transform.root.gameObject.GetComponent<sPlayer>();
                Vector2 ratio = new Vector2((pOther.gameObject.transform.position.x - gameObject.transform.position.x) / 
                                    (Mathf.Abs(pOther.gameObject.transform.position.y - gameObject.transform.position.y) + Mathf.Abs(pOther.gameObject.transform.position.x - gameObject.transform.position.x)), 
                    (pOther.gameObject.transform.position.y - gameObject.transform.position.y) / 
                                    (Mathf.Abs(pOther.gameObject.transform.position.y - gameObject.transform.position.y) + Mathf.Abs(pOther.gameObject.transform.position.x - gameObject.transform.position.x)));

                pOther.rb.velocity.Set(
                    pOther.rb.velocity.x + (ratio.x * Mathf.Sqrt(pOther.GetDamage() / dmg)),
                    pOther.rb.velocity.y + (ratio.y * Mathf.Sqrt(pOther.GetDamage() / dmg)), 0);
                rb.velocity.Set(
                    rb.velocity.x + (ratio.x * Mathf.Sqrt(dmg / pOther.GetDamage())),
                    rb.velocity.y + (ratio.y * Mathf.Sqrt(dmg / pOther.GetDamage())), 0);
            }
        }
        else
        {
            Debug.Log("Player " + pNumber + " triggered with " + collision.collider.material.name);

            if (collision.collider.material.name.Contains("pmStage"))
            {
                onStage = true;
                jumps = maxJumps;
                airborne = false;
                fastfall = false;
            }

            if (collision.collider.material.name.Contains("pmWall"))
            {
                onWall = true;
                jumps++;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.material.name.Contains("pmStage"))
        {
            onStage = false;
            airborne = true;
            jumps--;
        }

        if (collision.collider.material.name.Contains("pmWall"))
        {
            onWall = false;
        }
    }

    public void Spawn()
    {
        rig = gameObject.transform.GetChild(0).gameObject;
        rb = gameObject.GetComponent<Rigidbody>();
        box = gameObject.GetComponent<BoxCollider>();
        box.size = new Vector3(3, 9, 3.5f); box.center = new Vector3 (0, 4.5f, 0);
        zStart = transform.position.z;

        jumps = maxJumps - 1;
        stun = 0;
        actable = true;
        airborne = true;
        fastfall = false;
        invincible = false;
        holdingPlayer = false;

        mVert = 0;

        input = sData.MoveType.none;
        _anim.Play("Airborne");
    }
    public void Spawn(string mTxt, string cTxt)
    {
        SetPlayerMoves(mTxt);
        ctrlProfile = cTxt;
        Spawn();
    }
    public void Spawn(int pNum, string mTxt, string cTxt, Color col, int dir)
    {
        pNumber = pNum;
        orientation = dir;

        SetPlayerMoves(mTxt);
        ctrlProfile = cTxt;
        Spawn();
        SetColor(col);
    }

    public Animator GetCharAnimator { get { return _anim; } }
    public List<PlayerSensorData> GetPlayerSensors { get { return _sensors; } }

    public Dictionary<sData.MoveType, float[,]> GetPlayerMoves { get { return moves; } }
    private void SetPlayerMoves()
    {//Default Moveset
        moves = new Dictionary<sData.MoveType, float[,]>();
        moves.Add(sData.MoveType.jab, sData.GetMove("Vanilla Punch"));
    }
    public void SetPlayerMoves (string txt)
    {
        moves = sData.ReadMoves(txt);
    }
    public void SetPlayerMoves (Dictionary<sData.MoveType, float[,]> m)
    {
        moves = m;
    }

    public void SetControlProfile (string txt)
    {
        ctrlProfile = txt;
    }
    public void SetColor (Color c)
    {
        pColor = c;
        rig.GetComponent<Renderer>().material.color = pColor;
    }

    public sData.MoveType GetInput()
    {
        return input;
    }
    public void SendInput(sData.MoveType buf)
    {
        input = buf;
    }
    public void ClearInput()
    {
        input = sData.MoveType.none;
    }
    public bool NoInput()
    {
        if (input == sData.MoveType.none) return true;
        else return false;
    }

    public float GetDamage()
    {
        return dmg;
    }
    public void SetDamage(float d)
    {
        dmg = d;
    }
    public void ModDamage(float d)
    {
        dmg += d;
    }

    public bool CanJump()
    {
        if (jumps > 0) { return true; }
        else return false;
    }

    public void ModOrientation()
    {
        orientation = -orientation;
    }

    public bool OnStage()
    {
        return onStage;
    }
    public bool OnWall()
    {
        return onWall;
    }
    public bool IsAirborne()
    {
        return airborne;
    }
    public void ModAirborne()
    {
        airborne = !airborne;
    }
    public void SetAirborne (bool a)
    {
        airborne = a;
    }
    public bool IsActable()
    {
        return actable;
    }
    public void ModActable()
    {
        actable = !actable;
    }
    public void SetActable (bool state)
    {
        actable = state;
    }
    public int GetStun()
    {
        return stun;
    }
    public void SetStun (int s)
    {
        stun = s;
    }
    public bool IsHoldingPlayer()
    {
        return holdingPlayer;
    }
    public void ModHoldingPlayer()
    {
        holdingPlayer = !holdingPlayer;
    }

    override public string ToString()
    {
        string s=
            "Player: " + pNumber + '\n' +
            "Sensors: " + _sensors.Count + '\n'; 
                foreach (PlayerSensorData sensor in _sensors)
                {
                    s+= "   " + sensor.GetAtkType + '\n';
                }
        s+= "Moves: " + moves.Count + '\n';
                foreach (sData.MoveType move in moves.Keys)
                {
                    s+= "   " + move + '\n';
                }
        s+= "Controls Profile: " + ctrlProfile + '\n' +
            "Rigidbody Coordinates:   X:" + rb.position.x + "   Y:" + rb.position.y + '\n' +
            "Collider Dimensions:   X:" + box.size.x + "   Y:" + box.size.y + "   Z:" + box.size.z + '\n' +
            "Collider Height: " + box.center.y + '\n' +
            "Color: " + pColor + '\n' +
            "Animator: " + _anim.name + '\n' +
            "Animation: " + _anim.GetCurrentAnimatorClipInfo(0)[0].clip.name + '\n' +
            "Location State: ";
                if (airborne && !(onStage || onWall)) { s += "Airborne"; }
                else if (onStage && !(airborne || onWall)) { s += "On Stage"; }
                else if (onWall && !onStage) { s += "On Wall"; }
                else { s+= "Unknown"; }
        s+= '\n' + "Velocity: X=" + rb.velocity.x + "   Y=" + rb.velocity.y + '\n' +
            "Momentum Y=" + mVert + '\n' +
            "Orientation: " + orientation + '\n' +
            "Max Jumps: " + maxJumps + "   Current Jumps: " + jumps + '\n' +
            "Actable: " + actable + '\n' +
            "Fastfall: " + fastfall + '\n' +
            "Invincible: " + invincible + '\n' +
            "Holding Player: " + holdingPlayer + '\n';

        return s;
    }
}

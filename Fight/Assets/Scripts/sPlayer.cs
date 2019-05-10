using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerSensorData
{
    [SerializeField] sSensor[] _sensors;
    [SerializeField] sUtil.MoveType _atkType;

    public sSensor[] GetSensors { get { return _sensors; } }
    public sUtil.MoveType GetAtkType { get { return _atkType; } }
}

public class sPlayer : MonoBehaviour
{
    [SerializeField] Animator _anim;
    [SerializeField] List<PlayerSensorData> _sensors;

    public Dictionary<sUtil.MoveType, float[,]> moves;
    public string ctrlProfile;
    private GameObject rig;
    private Rigidbody rb;

    //Characters state
    public int orientation; //1 for facing right, -1 for facing left.
    public int maxJumps;
    int jumps;
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

    //State modifications and scaling
    private static float gMoveMod = 5;   //Scales the speed of grounded movement
    private static float aMoveMod = 4;    //Scales the speed of aerial movement
    private static float jHeightMod = 15;  //Scales the force of jumping
    private static float ffMod = 2;       //Scales the force of fastfalling

    //Current momentum
    private float mHorz;     //Horizontal force
    private float mVert;     //Vertical force

    private sUtil.MoveType input;

    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    private void Update()
    {
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
            if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime == 1)
            {
                actable = true;
                if (Input.GetKey(gameObject.GetComponent<sInput>().GetControls().jump)) { Jump('f'); }
                else { Jump('s'); }
            }
        }
        else
        {
            actable = false;
        }

        //Action processing
        if (IsActable())
        {
            //Queued input from buffer
            if (input != sUtil.MoveType.none)
            {
                //Debug.Log("Action pulled from buffer: " + input + "   Update #" + debugFrames);

                mHorz = 0;

                ////////////////////
                //Possible actions//
                ////////////////////

                if(airborne)
                {
                    //Bufferable aerial move processing
                    if (input == sUtil.MoveType.jump) { _anim.Play("AirJump"); Jump('a'); }
                    else if (input == sUtil.MoveType.nAir) { _anim.Play("NeutralAir"); }
                    else if (input == sUtil.MoveType.fAir) { _anim.Play("ForwardAir"); }
                    else if (input == sUtil.MoveType.bAir) { _anim.Play("BackAir"); }
                    else if (input == sUtil.MoveType.uAir) { _anim.Play("UpAir"); }
                    else if (input == sUtil.MoveType.dAir) { _anim.Play("DownAir"); }
                    else if (input == sUtil.MoveType.nSpec) { _anim.Play("NeutralSpecial"); }
                    else if (input == sUtil.MoveType.fSpec) { _anim.Play("ForwardSpecial"); }
                    else if (input == sUtil.MoveType.uSpec) { _anim.Play("UpSpecial"); }
                    else if (input == sUtil.MoveType.dSpec) { _anim.Play("DownSpecial"); }
                    else if (input == sUtil.MoveType.airdodge
                            || input == sUtil.MoveType.fRoll
                            || input == sUtil.MoveType.bRoll
                            || input == sUtil.MoveType.tech
                            || input == sUtil.MoveType.techHop) { _anim.Play("AirDodge"); }
                }
                else
                {
                    //Bufferable grounded move processing
                    if (input == sUtil.MoveType.jump) { _anim.Play("JumpSquat"); Debug.Log("Preparing for jump"); }
                    else if (input == sUtil.MoveType.jab) { _anim.Play("Jab"); }
                    else if (input == sUtil.MoveType.fLight) { _anim.Play("ForwardLight"); }
                    else if (input == sUtil.MoveType.uLight) { _anim.Play("UpLight"); }
                    else if (input == sUtil.MoveType.dLight) { _anim.Play("DownLight"); }
                    else if (input == sUtil.MoveType.fStrong) { _anim.Play("ForwardStrong"); }
                    else if (input == sUtil.MoveType.uStrong) { _anim.Play("UpStrong"); }
                    else if (input == sUtil.MoveType.dStrong) { _anim.Play("DownStrong"); }
                    else if (input == sUtil.MoveType.nSpec) { _anim.Play("NeutralSpecial"); }
                    else if (input == sUtil.MoveType.fSpec) { _anim.Play("ForwardSpecial"); }
                    else if (input == sUtil.MoveType.uSpec) { _anim.Play("UpSpecial"); }
                    else if (input == sUtil.MoveType.dSpec) { _anim.Play("DownSpecial"); }
                    else if (input == sUtil.MoveType.dodge) { _anim.Play("SpotDodge"); }
                    else if (input == sUtil.MoveType.fRoll) { _anim.Play("ForwardRoll"); }
                    else if (input == sUtil.MoveType.bRoll) { _anim.Play("BackwardRoll"); }
                    else if (input == sUtil.MoveType.grab) { _anim.Play("Grabbing"); }
                }

                //Buffered action executed
                gameObject.GetComponent<sInput>().ResetBuffer();
            }
            //Process non-bufferable actions (like movement)
            else
            {
                mHorz = 0;

                if (airborne) //Aerial control
                {
                    if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Airborne")) _anim.Play("Airborne");

                    if (!fastfall)
                    {
                        if (Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveVert) < .8) { fastfall = true; mVert = -fallSpeed * ffMod; }
                        else mVert = -fallSpeed;
                    }
                    else mVert = -fallSpeed * ffMod;

                    //Key Movement
                    if (Input.GetKey(gameObject.GetComponent<sInput>().GetControls().right))
                    {
                        transform.position += transform.right * orientation * moveSpeed * (aMoveMod / 1000);
                    }
                    else if (Input.GetKey(gameObject.GetComponent<sInput>().GetControls().left))
                    {
                        transform.position -= transform.right * orientation * moveSpeed * (aMoveMod / 1000);
                    }
                    else //Analog Movement
                    {
                        if (Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveHorz) > 0)
                        {
                            transform.position += transform.right * orientation * moveSpeed * (aMoveMod / 1000) * Mathf.Abs(Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveHorz));
                        }
                        else if (Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveHorz) < 0)
                        {
                            transform.position -= transform.right * orientation * moveSpeed * (aMoveMod / 1000) * Mathf.Abs(Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveHorz));
                        }
                    }
                }
                else //Grounded actions
                {
                    mVert = -10;

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
                        if (orientation != 1 || !_anim.GetCurrentAnimatorStateInfo(0).IsName("Running"))
                        {
                            orientation = 1;
                            _anim.Play("Running");
                        }
                    }
                    else if (Input.GetKey(gameObject.GetComponent<sInput>().GetControls().left))
                    {
                        if (orientation != -1 || !_anim.GetCurrentAnimatorStateInfo(0).IsName("Running"))
                        {
                            orientation = -1;
                            _anim.Play("Running");
                        }
                    }
                    //Analog Movement
                    else if (Mathf.Abs(Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveHorz)) > Mathf.Abs(Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveVert)))
                    {
                        if (Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveHorz) > 0) { orientation = 1; }
                        else { orientation = -1; }

                        if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Running")) _anim.Play("Running");
                        _anim.speed = Mathf.Abs(Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveHorz));

                        transform.position += transform.right * moveSpeed * (gMoveMod / 1000) * Mathf.Abs(Input.GetAxis(gameObject.GetComponent<sInput>().GetControls().moveHorz));
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
                if (input == sUtil.MoveType.airdodge
                                || input == sUtil.MoveType.fRoll
                                || input == sUtil.MoveType.bRoll
                                || input == sUtil.MoveType.tech
                                || input == sUtil.MoveType.techHop)
                {
                    if (onStage)
                    {
                        if (input == sUtil.MoveType.fRoll) { _anim.Play("ForwardRoll"); }
                        else if (input == sUtil.MoveType.bRoll) { _anim.Play("BackwardRoll"); }
                        else if (input == sUtil.MoveType.techHop) { _anim.Play("TechHop"); }
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
                if (input == sUtil.MoveType.fRoll) { _anim.Play("ForwardRoll"); }
                else if (input == sUtil.MoveType.bRoll) { _anim.Play("BackwardRoll"); }
                else if (input == sUtil.MoveType.getup) { _anim.Play("Getup"); }
                else if (input == sUtil.MoveType.getupAtk) { _anim.Play("GetupAttack"); }
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
        /**if (input == sUtil.MoveType.jab && (_anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || _anim.GetCurrentAnimatorStateInfo(0).IsName("Running")))
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
            gameObject.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if (orientation == -1)
        {
            gameObject.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }

        rb.AddForce(new Vector3(mHorz, mVert, 0.0f));
    }

    public void Jump(char type)
    {
        Debug.Log("Attempting Jump");

        //Short jump
        if (type == 's')
        {
            mVert = jumpSpeed * jHeightMod / 1.5f;
        }
        //Full jump
        else if (type == 'f')
        {
            mVert = jumpSpeed * jHeightMod;
        }
        //Aerial jump
        else if (type == 'a')
        {
            rb.velocity = new Vector3(rb.velocity.x, 0.0f, 0.0f);
            mVert = airjumpSpeed * jHeightMod;
        }

        jumps--;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "TopPlane" || collision.gameObject.name == "Platform")
        {
            onStage = true;
            jumps = maxJumps;
            airborne = false;
            fastfall = false;
        }

        if (collision.gameObject.name == "LeftPlane" || collision.gameObject.name == "RightPlane")
        {
            onWall = true;
            jumps++;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "TopPlane" || collision.gameObject.name == "Platform")
        {
            onStage = false;
            airborne = true;
        }

        if (collision.gameObject.name == "LeftPlane" || collision.gameObject.name == "RightPlane")
        {
            onWall = false;
        }
    }

    public void Spawn()
    {
        rig = gameObject.transform.GetChild(0).gameObject;
        rb = gameObject.GetComponent<Rigidbody>();

        jumps = maxJumps - 1;
        stun = 0;
        actable = true;
        airborne = true;
        fastfall = false;
        invincible = false;
        holdingPlayer = false;

        mVert = 0;
        mHorz = 0;

        input = sUtil.MoveType.none;
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

    public Dictionary<sUtil.MoveType, float[,]> GetPlayerMoves { get { return moves; } }
    private void SetPlayerMoves()
    {//Default Moveset
        moves = new Dictionary<sUtil.MoveType, float[,]>();
        moves.Add(sUtil.MoveType.jab, _sMoveData.GetFrameData["Vanilla Punch"]);
    }
    public void SetPlayerMoves (string txt)
    {
        moves = sUtil.ReadMoves(txt);
    }
    public void SetPlayerMoves (Dictionary<sUtil.MoveType, float[,]> m)
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
    public bool IsFastfall()
    {
        return fastfall;
    }
    public void ModFastfall()
    {
        fastfall = !fastfall;
        if (!fastfall) { mVert = fallSpeed; }
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
                foreach (sUtil.MoveType move in moves.Keys)
                {
                    s+= "   " + move + '\n';
                }
        s+= "Controls Profile: " + ctrlProfile + '\n' +
            "Rigidbody Coordinates:   X:" + rb.position.x + "   Y:" + rb.position.y + '\n' +
            "Color: " + pColor + '\n' +
            "Animator: " + _anim.name + '\n' +
            "Animation: " + _anim.GetCurrentAnimatorClipInfo(0)[0].clip.name + '\n' +
            "Location State: ";
                if (airborne && !(onStage || onWall)) { s += "Airborne"; }
                else if (onStage && !(airborne || onWall)) { s += "On Stage"; }
                else if (onWall && !onStage) { s += "On Wall"; }
                else { s+= "Unknown"; }
        s+= '\n' + "Velocity: X=" + rb.velocity.x + "   Y=" + rb.velocity.y + '\n' +
            "Momentum: X=" + mHorz + "   Y=" + mVert + '\n' +
            "Orientation: " + orientation + '\n' +
            "Max Jumps: " + maxJumps + "   Current Jumps: " + jumps + '\n' +
            "Actable: " + actable + '\n' +
            "Fastfall: " + fastfall + '\n' +
            "Invincible: " + invincible + '\n' +
            "Holding Player: " + holdingPlayer + '\n';

        return s;
    }
}

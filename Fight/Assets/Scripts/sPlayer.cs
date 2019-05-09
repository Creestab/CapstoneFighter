using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerSensorData
{
    [SerializeField] sSensor[] _sensors;
    [SerializeField] sPlayer.enumMoves _atkType;

    public sSensor[] GetSensors { get { return _sensors; } }
    public sPlayer.enumMoves GetAtkType { get { return _atkType; } }
}

public class sPlayer : MonoBehaviour
{
    [SerializeField] Animator _anim;
    [SerializeField] List<PlayerSensorData> _sensors;

    public Dictionary<enumMoves, float[,]> _moves;
    public TextAsset ctrlProfile;
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
    public float moveSpeed;
    public float jumpSpeed;
    public float airjumpSpeed;
    public float fallSpeed;

    //State modifications and scaling
    private static float gMoveMod = 5;   //Scales the speed of grounded movement
    private static float aMoveMod = 4;    //Scales the speed of aerial movement
    private static float jHeightMod = 15;  //Scales the force of jumping
    private static float ffMod = 10;       //Scales the force of fastfalling

    //Current momentum
    private float mHorz;     //Horizontal force
    private float mVert;     //Vertical force

    private enumMoves input;
    private int debugFrames;

    // Start is called before the first frame update
    void Start()
    {
        debugFrames = 0;
        rb = gameObject.GetComponent<Rigidbody>();

        SetPlayerMoves();

        jumps = maxJumps - 1;
        stun = 0;
        actable = true;
        airborne = true;
        fastfall = false;
        invincible = false;
        holdingPlayer = false;

        mVert = 0;
        mHorz = 0;

        _anim.Play("Airborne");
    }

    private void Update()
    {
        debugFrames++;

        //Reset animation speed when not running
        if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Running")) { _anim.speed = 1; }
        //Process current state
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || _anim.GetCurrentAnimatorStateInfo(0).IsName("Running")
                || _anim.GetCurrentAnimatorStateInfo(0).IsName("Crouching") || _anim.GetCurrentAnimatorStateInfo(0).IsName("Blocking"))
        {
            //Drops shield when not holding the input anymore
            if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Blocking") && Input.GetKeyUp(gameObject.GetComponent<sInput>().GetControls.block)) { _anim.Play("Idle"); }

            actable = true;
        }
        else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Airborne") || _anim.GetCurrentAnimatorStateInfo(0).IsName("AirJump"))
        {
            actable = true;
        }
        else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("JumpSquat"))
        {
            actable = true;
            if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime == 1)
            {
                if (Input.GetKey(gameObject.GetComponent<sInput>().GetControls.jump)) { Jump('f'); }
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
            if (input != enumMoves.none)
            {
                //Debug.Log("Action pulled from buffer: " + input + "   Update #" + debugFrames);

                ////////////////////
                //Possible actions//
                ////////////////////
                if (!IsAirborne())
                {
                    //Bufferable grounded move processing
                    if (input == enumMoves.jump) { GetCharAnimator.Play("JumpSquat"); }
                    else if (input == enumMoves.jab) { GetCharAnimator.Play("Jab"); }
                    else if (input == enumMoves.fLight) { GetCharAnimator.Play("ForwardLight"); }
                    else if (input == enumMoves.uLight) { GetCharAnimator.Play("UpLight"); }
                    else if (input == enumMoves.dLight) { GetCharAnimator.Play("DownLight"); }
                    else if (input == enumMoves.fStrong) { GetCharAnimator.Play("ForwardStrong"); }
                    else if (input == enumMoves.uStrong) { GetCharAnimator.Play("UpStrong"); }
                    else if (input == enumMoves.dStrong) { GetCharAnimator.Play("DownStrong"); }
                    else if (input == enumMoves.nSpec) { GetCharAnimator.Play("NeutralSpecial"); }
                    else if (input == enumMoves.fSpec) { GetCharAnimator.Play("ForwardSpecial"); }
                    else if (input == enumMoves.uSpec) { GetCharAnimator.Play("UpSpecial"); }
                    else if (input == enumMoves.dSpec) { GetCharAnimator.Play("DownSpecial"); }
                    else if (input == enumMoves.dodge) { GetCharAnimator.Play("SpotDodge"); }
                    else if (input == enumMoves.fRoll) { GetCharAnimator.Play("ForwardRoll"); }
                    else if (input == enumMoves.bRoll) { GetCharAnimator.Play("BackwardRoll"); }
                    else if (input == enumMoves.grab) { GetCharAnimator.Play("Grabbing"); }
                }
                else
                {
                    //Bufferable aerial move processing
                    if (input == enumMoves.jump) { GetCharAnimator.Play("AirJump"); Jump('a'); }
                    else if (input == enumMoves.nAir) { GetCharAnimator.Play("NeutralAir"); }
                    else if (input == enumMoves.fAir) { GetCharAnimator.Play("ForwardAir"); }
                    else if (input == enumMoves.bAir) { GetCharAnimator.Play("BackAir"); }
                    else if (input == enumMoves.uAir) { GetCharAnimator.Play("UpAir"); }
                    else if (input == enumMoves.dAir) { GetCharAnimator.Play("DownAir"); }
                    else if (input == enumMoves.nSpec) { GetCharAnimator.Play("NeutralSpecial"); }
                    else if (input == enumMoves.fSpec) { GetCharAnimator.Play("ForwardSpecial"); }
                    else if (input == enumMoves.uSpec) { GetCharAnimator.Play("UpSpecial"); }
                    else if (input == enumMoves.dSpec) { GetCharAnimator.Play("DownSpecial"); }
                    else if (input == enumMoves.airdodge
                            || input == enumMoves.fRoll
                            || input == enumMoves.bRoll
                            || input == enumMoves.tech
                            || input == enumMoves.techHop) { GetCharAnimator.Play("AirDodge"); }
                }

                //Buffered action executed
                gameObject.GetComponent<sInput>().ResetBuffer();
            }
            //Process non-bufferable actions (like movement)
            else
            {
                //Grounded actions
                if (!IsAirborne())
                {
                    //Grab actions
                    if (IsHoldingPlayer())
                    {

                    }
                    //Enter Block
                    if (Input.GetKeyDown(gameObject.GetComponent<sInput>().GetControls.block) && !GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Blocking")) { GetCharAnimator.Play("Blocking"); }
                    //Key movement
                    else if (Input.GetKey(gameObject.GetComponent<sInput>().GetControls.down))
                    {
                        if (!GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Crouching")) { GetCharAnimator.Play("Crouching"); }
                    }
                    else if (Input.GetKey(gameObject.GetComponent<sInput>().GetControls.right))
                    {
                        if (orientation != 1 || !GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
                        {
                            orientation = 1;
                            GetCharAnimator.Play("Running");
                        }
                    }
                    else if (Input.GetKey(gameObject.GetComponent<sInput>().GetControls.left))
                    {
                        if (orientation != -1 || !GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
                        {
                            orientation = -1;
                            GetCharAnimator.Play("Running");
                        }
                    }
                    //Analog Movement
                    else if (Mathf.Abs(Input.GetAxis(gameObject.GetComponent<sInput>().GetControls.moveHorz)) > Mathf.Abs(Input.GetAxis(gameObject.GetComponent<sInput>().GetControls.moveVert)))
                    {
                        GetCharAnimator.Play("Running");
                    }
                    else if (Input.GetAxis(gameObject.GetComponent<sInput>().GetControls.moveVert) < -.5)
                    {
                        //Crouch
                        GetCharAnimator.Play("Crouching");
                    }
                    else
                    {
                        GetCharAnimator.Play("Idle");
                    }
                }
                else if (!GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Airborne")) //Aerial control
                {
                    GetCharAnimator.Play("Airborne");
                    //Check for fastfall
                    if (!IsFastfall() && Input.GetAxis(gameObject.GetComponent<sInput>().GetControls.moveVert) < .9) { ModFastfall(); }
                }
            }
        }
        //Process influence to inactable states (Teching, DI, mashing, ext.)
        else
        {
            //Process DI
            if (IsAirborne())
            {

            }

            //Process teching
            if (OnStage() && IsAirborne() && (input == enumMoves.airdodge
                            || input == enumMoves.fRoll
                            || input == enumMoves.bRoll
                            || input == enumMoves.tech
                            || input == enumMoves.techHop))
            {
                if (input == enumMoves.fRoll) { GetCharAnimator.Play("ForwardRoll"); }
                else if (input == enumMoves.bRoll) { GetCharAnimator.Play("BackwardRoll"); }
                else if (input == enumMoves.techHop) { GetCharAnimator.Play("TechHop"); }
                else { GetCharAnimator.Play("TechInPlace"); }

                ModAirborne();
                ModActable();
                gameObject.GetComponent<sInput>().ResetBuffer();
            }
            else if (OnWall() && (input == enumMoves.airdodge
                            || input == enumMoves.fRoll
                            || input == enumMoves.bRoll
                            || input == enumMoves.tech
                            || input == enumMoves.techHop))
            {
                GetCharAnimator.Play("WallJump");

                ModActable();
                gameObject.GetComponent<sInput>().ResetBuffer();
            }
            else if (!IsAirborne() && !GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("KnockedDown"))
            {
                GetCharAnimator.Play("KnockedDown");
            }

            //Process actions out of knockdown
            if (GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("KnockedDown"))
            {
                if (input == enumMoves.fRoll) { GetCharAnimator.Play("ForwardRoll"); }
                else if (input == enumMoves.bRoll) { GetCharAnimator.Play("BackwardRoll"); }
                else if (input == enumMoves.getup) { GetCharAnimator.Play("Getup"); }
                else if (input == enumMoves.getupAtk) { GetCharAnimator.Play("GetupAttack"); }
            }
        }

        //Debuging using moves after actable states
        /*if (input == enumMoves.jab && (GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || GetCharAnimator.GetCurrentAnimatorStateInfo(0).IsName("Running")))
        {
            Debug.Log("Should be able to jab");
            if(isActable()) { Debug.Log( "Actable"); }
            else { Debug.Log("Error: Not Actable when should be actable"); }
        }
        else if(Input.GetKeyUp(gameObject.GetComponent<sInput>().GetControls.right) || Input.GetKeyUp(controls.left))
        {
            Debug.Log("Stopped moving with no jab in buffer");
            if (actable) { Debug.Log("Actable"); }
            else { Debug.Log("Error: Not Actable when should be actable"); }
        }*/

        //Process in air motion
        if (airborne)
        {
            if (fastfall && mVert < fallSpeed * ffMod) { mVert = -fallSpeed * ffMod; }
            else mVert = -fallSpeed;
        }
        else { mVert = 0; rb.velocity = new Vector3(rb.velocity.x, 0, 0); }

        //Process grounded movement
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Running"))
        {
            _anim.speed = Mathf.Abs(Input.GetAxis(gameObject.GetComponent<sInput>().GetControls.moveHorz));
            if (Input.GetAxis(gameObject.GetComponent<sInput>().GetControls.moveHorz) > 0)
            {
                orientation = 1;
                transform.position -= transform.right * moveSpeed * (gMoveMod / 1000) * Mathf.Abs(Input.GetAxis(gameObject.GetComponent<sInput>().GetControls.moveHorz));
            }
            else if (Input.GetAxis(gameObject.GetComponent<sInput>().GetControls.moveHorz) < 0)
            {
                orientation = -1;
                transform.position -= transform.right * moveSpeed * (gMoveMod / 1000) * Mathf.Abs(Input.GetAxis(gameObject.GetComponent<sInput>().GetControls.moveHorz));
            }
        }
        //Process orientation
        if (orientation == 1)
        {
            gameObject.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if (orientation == -1)
        {
            gameObject.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }

        Vector3 movement = new Vector3(mHorz, mVert, 0.0f);

        rb.AddForce(movement);
    }

    public void Jump(char type)
    {
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

    /*   public struct MoveData
       {
           public enumMoves moveSlot;
           public string moveName;
           public float[,] frameData;

           public MoveData(enumMoves slot, string name)
           {
               moveSlot = slot;
               moveName = name;
               frameData = _sMoveData.FrameData[name];
           }
       }*/

    private void OnCollisionEnter(Collision collision)
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

    private void OnCollisionExit(Collision collision)
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

    public enum enumMoves
    {
        none,
        jump,
        jab,
        uLight,
        fLight,
        dLight,
        uStrong,
        fStrong,
        dStrong,
        nAir,
        uAir,
        fAir,
        dAir,
        bAir,
        nSpec,
        uSpec,
        fSpec,
        dSpec,
        grab,
        pummel,
        uThrow,
        fThrow,
        dThrow,
        bThrow,
        getup,
        getupAtk,
        shield,
        tech,
        techHop,
        fRoll,
        bRoll,
        dodge,
        airdodge
    }

    public Animator GetCharAnimator { get { return _anim; } }
    public List<PlayerSensorData> GetPlayerSensors { get { return _sensors; } }

    public Dictionary<enumMoves, float[,]> GetPlayerMoves { get { return _moves; } }
    private void SetPlayerMoves()
    {
        //Default Movesets
        _moves = new Dictionary<enumMoves, float[,]>();
        _moves.Add(enumMoves.jab, _sMoveData.GetFrameData["Vanilla Punch"]);
    }
    public void SetPlayerMoves (Dictionary<enumMoves, float[,]> moves)
    {
        _moves = moves;
    }
    public void SetPlayerMoves (TextAsset moves)
    {
        StreamReader reader = new StreamReader("Assets/Text/" + moves.name + ".txt");

        _moves = new Dictionary<enumMoves, float[,]>();
        _moves.Add(enumMoves.jab, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
        _moves.Add(enumMoves.uLight, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
        _moves.Add(enumMoves.fLight, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
        _moves.Add(enumMoves.dLight, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
        _moves.Add(enumMoves.uStrong, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
        _moves.Add(enumMoves.fStrong, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
        _moves.Add(enumMoves.dStrong, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
        _moves.Add(enumMoves.nAir, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
        _moves.Add(enumMoves.uAir, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
        _moves.Add(enumMoves.fAir, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
        _moves.Add(enumMoves.dAir, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
        _moves.Add(enumMoves.bAir, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
        _moves.Add(enumMoves.nSpec, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
        _moves.Add(enumMoves.uSpec, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
        _moves.Add(enumMoves.fSpec, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
        _moves.Add(enumMoves.dSpec, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);

        Debug.Log("Player " + pNumber + " Jab Frames: " + (_moves[enumMoves.jab][0,1] + 1));
    }

    public void SetControlProfile (TextAsset ctrl)
    {
        ctrlProfile = ctrl;
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
            "Sensors: " + '\n'; 
                foreach (PlayerSensorData sensor in _sensors)
                {
                    s+= "   " + sensor.GetAtkType + '\n';
                }
        s+= "Moves: " + '\n';
                foreach (enumMoves move in _moves.Keys)
                {
                    s+= "   " + move + '\n';
                }
        s+= "Controls Profile: " + ctrlProfile.name + '\n' +
            "Rigidbody Coordinates:   X:" + rb.position.x + "   Y:" + rb.position.y + '\n' +
            "Animator: " + _anim.name + '\n' +
            "Animation: " + _anim.GetCurrentAnimatorClipInfo(0)[0].clip.name + '\n' +
            "Location State: ";
                if (airborne && !(onStage || onWall)) { s += "Airborne"; }
                else if (onStage && !(airborne || onWall)) { s += "On Stage"; }
                else if (onWall && !onStage) { s += "On Wall"; }
                else { s+= "Unknown"; }
        s+= "Velocity: X=" + rb.velocity.x + "   Y=" + rb.velocity.y + '\n' +
            "Momentum: X=" + mHorz + "   Y=" + mVert + '\n' +
            "Orientation: " + orientation + '\n' +
            "Max Jumps: " + maxJumps + "   Current Jumps: " + jumps + '\n' +
            "Actable: " + actable + '\n' +
            "Fastfall: " + fastfall + '\n' +
            "Invincible: " + invincible + '\n' +
            "Holding Player: " + holdingPlayer + '\n' + '\n' +
            "sPlayer Printed on Frame: " + debugFrames;

        return s;

        /*[SerializeField] Animator _anim;
        [SerializeField] List<PlayerSensorData> _sensors;

        public Dictionary<enumMoves, float[,]> _moves;
        public TextAsset ctrlProfile;
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
        public float moveSpeed;
        public float jumpSpeed;
        public float airjumpSpeed;
        public float fallSpeed;

        //State modifications and scaling
        private static float gMoveMod = 5;   //Scales the speed of grounded movement
        private static float aMoveMod = 4;    //Scales the speed of aerial movement
        private static float jHeightMod = 15;  //Scales the force of jumping
        private static float ffMod = 10;       //Scales the force of fastfalling

        //Current momentum
        private float mHorz;     //Horizontal force
        private float mVert;     //Vertical force*/
    }
}

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
    private sInput input;

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

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        input = gameObject.GetComponent<sInput>();

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

        //_anim.Play("Airborne");
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
            if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Blocking") && Input.GetKeyUp(input.GetControls.block)) { _anim.Play("Idle"); }

            actable = true;
            if (airborne) { ModAirborne(); }
        }
        else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Airborne") || _anim.GetCurrentAnimatorStateInfo(0).IsName("AirJump"))
        {
            actable = true;
            if (!airborne) { ModAirborne(); }
        }
        else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("JumpSquat"))
        {
            actable = true;
            if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime == 1)
            {
                if (Input.GetKey(input.GetControls.jump)) { Jump('f'); }
                else { Jump('s'); }
            }
        }
        else
        {
            actable = false;
        }

        //Process in air motion
        if(airborne)
        {
            if (fastfall && mVert < fallSpeed * ffMod) { mVert = -fallSpeed * ffMod; }
            else mVert = -fallSpeed;
        }
        else { mVert = 0; }

        //Process grounded movement
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Running"))
        {
            _anim.speed = Mathf.Abs(Input.GetAxis(input.GetControls.moveHorz));
            if (Input.GetAxis(input.GetControls.moveHorz) > 0)
            {
                orientation = 1;
                transform.position -= transform.right * moveSpeed * (gMoveMod / 1000) * Mathf.Abs(Input.GetAxis(input.GetControls.moveHorz));
            }
            else if (Input.GetAxis(input.GetControls.moveHorz) < 0)
            {
                orientation = -1;
                transform.position -= transform.right * moveSpeed * (gMoveMod / 1000) * Mathf.Abs(Input.GetAxis(input.GetControls.moveHorz));
            }
        }
        //Process orientation
        if (orientation == 1)
        {
            gameObject.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if(orientation == -1)
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
        airdodge,
    }

    public Animator GetCharAnimator { get { return _anim; } }
    public List<PlayerSensorData> GetPlayerSensors { get { return _sensors; } }
    public Dictionary<enumMoves, float[,]> GetPlayerMoves {  get { return _moves; } }
    private void SetPlayerMoves()
    {    
        //Default Movesets
        _moves = new Dictionary<enumMoves, float[,]>();
        _moves.Add(enumMoves.jab, _sMoveData.GetFrameData["Vanilla Punch"]);
    }
    public void SetPlayerMoves(Dictionary<enumMoves, float[,]> moves)
    {
        _moves = moves;
    }
    public void SetPlayerMoves(TextAsset moves)
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
    public void SetActable(bool state)
    {
        actable = state;
    }
    public int GetStun()
    {
        return stun;
    }
    public void SetStun(int s)
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

}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class sData
{
    public enum InputAction
    {
        Move,
        Light,
        Heavy,
        Special,
        Block,
        Grab,
        Jump,
        Alt,
        Pause
    };
    public enum MoveType
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
    public enum ColliderState
    {
        None,
        HitBox,
        HurtBox
    };

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

        public override string ToString()
        {
            string s =
                "Input Buffer: " + buffer + '\n' +
                "Movement Axis (Horizonal): " + moveHorz + '\n' +
                "Movement Axis (Vertical): " + moveVert + '\n' +
                "Misc Axis (Horizontal): " + rHorz + '\n' +
                "Misc Axis (Vertical): " + rVert + '\n' +
                "Misc Axis Use: " + rStickUse + '\n' +
                "Binary Movement: " + '\n' + "   Left: " + left + "   Right: " + right + "   Up: " + up + "   Down: " + down + '\n' +
                "Light Attack: " + light + '\n' +
                "Heavy Attack: " + heavy + '\n' +
                "Light Input to Heavy Attack Threshold: " + lightToHeavy + '\n' +
                "Special: " + special + '\n' +
                "Block: " + block + '\n' +
                "Grab: " + grab + '\n' +
                "Jump: " + jump + '\n' +
                "Alter Input: " + alt + '\n' +
                "Pause: " + pause + '\n';

            return s;
        }
    }
    
    /// <summary>
    ///List of moves
    ///{{frame 0, ending frame, # of hit instances, special frame type (0 if none), special effect value (0 if none), special frame start, special frame end, rarity (for draft: 0 common, 1 uncommon, 2 rare)},
    ///{hitbox start frame, hitbox end frame, damage, knockback angle, knockback base, knockback growth, hitstun frames, priority}, ... } 
    /// </summary>
    public static Dictionary<string, float[,]> libMoves = new Dictionary<string, float[,]>()
    {
        //Jabs
        {"Vanilla Punch", new float[,]{{0,10,1,0,0,0,0,0}, {2,5,3,0,10,1,1,0}}},

        //Up Lights
        {"Vanilla Fist Pump", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Forward Lights
        {"Vanilla Arm Swipe", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Down Lights
        {"Vanilla Stomp", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Up Strongs
        {"Skyclap", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Forward Strongs
        {"This Is Sparta", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Down Strongs
        {"The Splits", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Neutral Aerials
        {"Sex Kick", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Up Aerials
        {"Bicycle Kick", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Forward Aerials
        {"Swat Away", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Down Aerials
        {"Sky Stomp", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Back Aerials
        {"Boneless", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Neutral Specials
        {"Twirl", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Up Specials
        {"Skycutter Leap", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Forward Specials
        {"Bullrush", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}},

        //Down Specials
        {"The Worm", new float[,]{{0,0,0,0,0,0,0,0}, {0,0,0,0,0,0,0,0}}}
    };

    /// <summary>  ///
    /// FUNCTIONS  ///
    /// </summary> ///

    public static Dictionary<string, float[,]> GetMoves
    {
        get { return libMoves; }
    }
    public static float[,] GetMove(string k)
    {
        return libMoves[k];
    }
    public static ControlScheme ReadControls(string name)
    {
        if (File.Exists("Assets/Text/" + name + ".txt"))
        {
            ControlScheme _controls = new ControlScheme();
            StreamReader reader = new StreamReader("Assets/Text/" + name + ".txt");

            _controls.buffer = int.Parse(reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
            _controls.moveHorz = reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1];
            _controls.moveVert = reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1];
            _controls.rStickUse = (InputAction)System.Enum.Parse(typeof(InputAction), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
            _controls.rHorz = reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1];
            _controls.rVert = reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1];
            _controls.left = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
            _controls.right = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
            _controls.up = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
            _controls.down = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
            _controls.light = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
            _controls.heavy = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
            _controls.lightToHeavy = float.Parse(reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
            _controls.special = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
            _controls.block = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
            _controls.grab = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
            _controls.jump = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
            _controls.alt = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);
            _controls.pause = (KeyCode)System.Enum.Parse(typeof(KeyCode), reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]);

            return _controls;
        }
        else throw new System.NullReferenceException("File not found");
    }

    public static Dictionary<MoveType, float[,]> ReadMoves(string name)
    {
        if (File.Exists("Assets/Text/" + name + ".txt"))
        {
            Dictionary<MoveType, float[,]> _moves = new Dictionary<MoveType, float[,]>();
            StreamReader reader = new StreamReader("Assets/Text/" + name + ".txt");

            _moves = new Dictionary<MoveType, float[,]>();
            _moves.Add(MoveType.jab, libMoves[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(MoveType.uLight, libMoves[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(MoveType.fLight, libMoves[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(MoveType.dLight, libMoves[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(MoveType.uStrong, libMoves[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(MoveType.fStrong, libMoves[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(MoveType.dStrong, libMoves[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(MoveType.nAir, libMoves[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(MoveType.uAir, libMoves[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(MoveType.fAir, libMoves[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(MoveType.dAir, libMoves[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(MoveType.bAir, libMoves[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(MoveType.nSpec, libMoves[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(MoveType.uSpec, libMoves[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(MoveType.fSpec, libMoves[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(MoveType.dSpec, libMoves[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);

            return _moves;
        }
        else throw new System.NullReferenceException("File not found");
    }
}

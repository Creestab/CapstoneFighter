using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class sUtil
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

            _moves = new Dictionary<sUtil.MoveType, float[,]>();
            _moves.Add(sUtil.MoveType.jab, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(sUtil.MoveType.uLight, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(sUtil.MoveType.fLight, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(sUtil.MoveType.dLight, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(sUtil.MoveType.uStrong, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(sUtil.MoveType.fStrong, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(sUtil.MoveType.dStrong, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(sUtil.MoveType.nAir, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(sUtil.MoveType.uAir, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(sUtil.MoveType.fAir, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(sUtil.MoveType.dAir, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(sUtil.MoveType.bAir, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(sUtil.MoveType.nSpec, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(sUtil.MoveType.uSpec, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(sUtil.MoveType.fSpec, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);
            _moves.Add(sUtil.MoveType.dSpec, _sMoveData.GetFrameData[reader.ReadLine().Split(new char[] { ' ' }, 2, System.StringSplitOptions.None)[1]]);

            return _moves;
        }
        else throw new System.NullReferenceException("File not found");
    }
}

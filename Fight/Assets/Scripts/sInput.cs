using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class sInput : MonoBehaviour
{
    public enum ControllerType { Keyboard, GameCube, XBox };
    public enum InputAction { Move, Light, Heavy, Special, Block, Grab, Jump, Alt, Pause };

    public struct ControlMapVJoy
    {
        public string moveHorz;
        public string moveVert;
        public InputAction rStick;
        public KeyCode lightAtk;
        public KeyCode heavyAtk;
        public KeyCode special;
        public KeyCode block;
        public KeyCode grab;
        public KeyCode jump;
        public KeyCode alt;
        public KeyCode pause;

        public ControlMapVJoy(sPlayer p) //default controls
        {
            if (p.pNumber == 1)
            {
                moveHorz = "P1_LHorz";
                moveVert = "P1_LVert";
                rStick = InputAction.Heavy;
                lightAtk = KeyCode.Joystick1Button0;
                heavyAtk = KeyCode.Joystick1Button2;
                special = KeyCode.Joystick1Button1;
                block = KeyCode.Joystick1Button6;
                grab = KeyCode.Joystick1Button4;
                jump = KeyCode.Joystick1Button3;
                alt = KeyCode.Joystick1Button5;
                pause = KeyCode.Joystick1Button7;
            }
            else
            {
                moveHorz = "P2_LHorz";
                moveVert = "P2_LVert";
                rStick = InputAction.Heavy;
                lightAtk = KeyCode.Joystick2Button0;
                heavyAtk = KeyCode.Joystick2Button2;
                special = KeyCode.Joystick2Button1;
                block = KeyCode.Joystick2Button6;
                grab = KeyCode.Joystick2Button4;
                jump = KeyCode.Joystick2Button3;
                alt = KeyCode.Joystick2Button5;
                pause = KeyCode.Joystick2Button7;
            }
        }
    }
    /*public struct ControlMapXInput
    {
    }
    public struct ControlMapKeyboard
    {
    }*/

    sPlayer pChar;
    [SerializeField] ControllerType device;
    ControlMapVJoy controls;

    List<KeyCode> inputs;
    int buffer;
    static int InputBuffer = 6;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;

        pChar = transform.parent.GetComponent<sPlayer>();
        controls = new ControlMapVJoy(pChar);

        inputs = new List<KeyCode>();
        buffer = 0;
    }

    private void FixedUpdate()
    {


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
            else if (Input.GetKeyDown(controls.lightAtk))
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

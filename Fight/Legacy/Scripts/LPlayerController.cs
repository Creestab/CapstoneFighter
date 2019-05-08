using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class PlayerController : MonoBehaviour {

    //Control bindings
    public string ctrlLeft;
    public string ctrlRight;
    public string ctrlUp;
    public string ctrlDown;
    public string ctrlJump;
    public string ctrlAtk;
    public string ctrlSpec;
    public string ctrlBlock;
    public string ctrlGrab;

    //Character base attributes
    public int playerNum;
    public Vector3 spawnPoint;
    public float moveSpeed;
    public float jumpSpeedGrounded;
    public float jumpSpeedAirborn;
    public int numAirJumps;

    //Assets
    private Rigidbody rb;

    //State modifications and scaling
    public int inputBuffer;
    private float gMoveMod = 5;   //Scales the speed of grounded movement
    private float aMoveMod = 4;    //Scales the speed of aerial movement
    private float jHeightMod = 15;  //Scales the force of jumping
    private float ffMod = 10;       //Scales the force of fastfalling
    private float slideMod = 1;    //Scales the friction on stage

    //Character states
    private bool actable;       //Does the player have control
    private bool airborn;       //Is the player in the air
    private bool fastfall;      //Is the player fastfalling
    private bool slide;         //Is the player sliding on stage
    private int frmsSlide=0;    //How long has the player been sliding
    private bool invincible;    //Is the player invincible
    private int frmsInvin=0;    //How many frames has the player been invincible
    private int jumps;           //How many jumps does the player have
    private int frmsSH=3;        //Frame window for shorthopping
    private float moveHorz;     //Horizontal force
    private float moveVert;     //Vertical force

    //Buffer tracking
    private int bfrJump = 0;
    private int bfrSH = 0;
    private int bfrAtk = 0;
    private int bfrSpec = 0;
    private int bfrBlock = 0;
    private int bfrGrab = 0;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        actable = true;
        airborn = true;
        fastfall = false;
        slide = false;
        invincible = false;
        jumps = numAirJumps;
        moveHorz = 0;
        moveVert = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        CharState();
        InputBuffer();


        //////////////////////////////
        //////MAIN CONTROLLER/////////
        //////////////////////////////
        if (Input.GetKey(ctrlRight)) //input move right
        {
            if (!actable && airborn) {moveHorz = moveSpeed * (aMoveMod / 10);} //Uses forces if airborn and inactible (influences knockback and special fall)
            if (actable && airborn) {transform.position += transform.right * moveSpeed * (aMoveMod / 1000); } //Uses transform if airborn and actable
            if (actable && !airborn) {transform.position += transform.right * moveSpeed * (gMoveMod / 1000);} //Uses transform if grounded
        }
        if (Input.GetKey(ctrlLeft)) //input move left
        {
            if (!actable && airborn) {moveHorz = -moveSpeed * (aMoveMod / 10); } //Uses forces if airborn and inactible (influences knockback and special fall)
            if (actable && airborn) {transform.position -= transform.right * moveSpeed * (aMoveMod / 1000); } //Uses transform if airborn and actable
            if (actable && !airborn) {transform.position -= transform.right * moveSpeed * (gMoveMod / 1000); } //Uses transform if grounded
        }

        //Processes only executable when actable
        if (actable)
        {
            //input jump if available
            if (bfrJump != 0 && jumps != 0) {Jump(); bfrJump = 0;} 

            //input fastfall
            if (Input.GetKey(ctrlDown) && !fastfall && airborn && rb.velocity.y <= 0)
            {
                moveVert = -jumpSpeedAirborn * ffMod;
                fastfall = true;
            }
        }

        Vector3 movement = new Vector3(moveHorz, moveVert, 0.0f);

        rb.AddForce(movement);
	}

    //////////////////////////////
    //////////METHODS/////////////
    //////////////////////////////

    //Character state processing
    private void CharState()
    {
        moveVert = 0;

        //slide state processing
        if (slide)
        {
            frmsSlide++;
            if (rb.velocity.z > 1) {moveHorz -= slideMod / frmsSlide;}
            else if (moveHorz < -1) {moveHorz += slideMod / frmsSlide;}
            else {moveHorz = 0; slide = false; frmsSlide = 0;}
        }
    }

    //Input buffer tracking
    private void InputBuffer()
    {
        //adjust buffers
        if (bfrJump > 0) {bfrJump--;}

        //jump input processing
        if (Input.GetKeyDown(ctrlJump))
        {
            bfrSH = 0;
            if (airborn) {bfrJump = inputBuffer;}
        }
        if (!airborn)
        {
            if (Input.GetKey(ctrlJump)) {bfrSH++;}
            if (Input.GetKeyUp(ctrlJump) && bfrSH <= frmsSH) {bfrJump = inputBuffer;}
            if (bfrSH > frmsSH) {bfrJump = inputBuffer;}
        }
    }

    private void Jump()
    {
        if (!airborn)
        {
            if (bfrSH > frmsSH) {moveVert = jumpSpeedGrounded * jHeightMod;}
            else {moveVert = jumpSpeedGrounded * jHeightMod / 1.5f;}
            bfrSH = 0;
            bfrJump = 0;
        }
        else
        {
            rb.velocity = new Vector3(rb.velocity.x, 0.0f, 0.0f);
            moveVert = jumpSpeedAirborn * jHeightMod;
            jumps--;
            bfrSH = 0;
            bfrJump = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Landing on stage
        if (collision.gameObject.name == "TopPlane" || collision.gameObject.name == "Platform")
        {
            jumps = numAirJumps + 1; //resets available jumps to full
            airborn = false;
            slide = true;
            fastfall = false;
        }

        if (collision.gameObject.name == "LeftPlane" || collision.gameObject.name == "RightPlane")
        {
            jumps = numAirJumps; //adds jumps from ledge
            airborn = false;
            slide = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //Eats grounded jump when not grounded
        if (collision.gameObject.name == "TopPlane" || collision.gameObject.name == "Platform")
        {
            jumps--;
            airborn = true;
            slide = false;
        }
    }
}
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //Character base attributes
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
    private float jHeightMod = 10;  //Scales the force of jumping
    private float ffMod = 10;       //Scales the force of fastfalling
    private float slideMod = 1;    //Scales the friction on stage

    //Character states
    private bool actable;       //Does the player have control
    private bool airborn;       //Is the player in the air
    private bool fastfall;      //Is the player fastfalling
    private bool slide;         //Is the player sliding on stage
    private int frmsSliding=0;  //How long has the player been sliding
    public int jumps;           //How many jumps does the player have
    private float moveHorz;     //Horizontal force
    private float moveVert;     //Vertical force

    //Button buffer tracking
    private int bfrJump = 0;

	// Use this for initialization
	void Start () {
        Application.targetFrameRate = 30;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        actable = true;
        airborn = true;
        fastfall = false;
        slide = false;
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
        if (Input.GetKey("d")) //Input move right
        {
            if (!actable && airborn) {moveHorz = moveSpeed * (aMoveMod / 10);} //Uses forces if airborn and inactible (influences knockback and special fall)
            if (actable && airborn) {transform.position += transform.right * moveSpeed * (aMoveMod / 1000); } //Uses transform if airborn and actable
            if (actable && !airborn) {transform.position += transform.right * moveSpeed * (gMoveMod / 1000);} //Uses transform if grounded
        }
        if (Input.GetKey("a")) //Input move left
        {
            if (!actable && airborn) {moveHorz = -moveSpeed * (aMoveMod / 10); } //Uses forces if airborn and inactible (influences knockback and special fall)
            if (actable && airborn) {transform.position -= transform.right * moveSpeed * (aMoveMod / 1000); } //Uses transform if airborn and actable
            if (actable && !airborn) {transform.position -= transform.right * moveSpeed * (gMoveMod / 1000); } //Uses transform if grounded
        }

        //Processes only executable when actable
        if (actable)
        {
            if (bfrJump != 0 && jumps != 0) { Jump(); } //Input jump if available

            if (Input.GetKeyDown("s")) //Input crouch or fastfall
            {
                if (airborn && rb.velocity.y <= 0) {moveVert = -jumpSpeedAirborn * ffMod;}
            }
        }

        Vector3 movement = new Vector3(moveHorz, moveVert, 0.0f);

        rb.AddForce(movement);
	}

    //Character state processing
    private void CharState()
    {
        moveVert = 0;

        //Slide state processing
        if (slide)
        {
            frmsSliding++;
            if (rb.velocity.z > 1) {moveHorz -= slideMod / frmsSliding;}
            else if (moveHorz < -1) {moveHorz += slideMod / frmsSliding;}
            else {moveHorz = 0; slide = false; frmsSliding = 0;}
        }
    }

    //Input buffer tracking
    private void InputBuffer()
    {
        //Jump buffer
        if(bfrJump > 0) {bfrJump--;}
        if(Input.GetKeyDown("space")) {bfrJump = inputBuffer;}
    }

    private void Jump()
    {
        if (!airborn) {moveVert = jumpSpeedGrounded * jHeightMod;}
        else
        {
            moveVert = jumpSpeedAirborn * jHeightMod;
            jumps--;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Stage") {jumps = numAirJumps + 1;} //Resets available jumps
        airborn = false;
        slide = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.name == "Stage") {jumps--;} //Resets available jumps
        airborn = true;
        slide = false;
    }
}

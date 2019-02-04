using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float moveSpeed;
    public float jumpSpeed;

    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        float MoveHorz = Input.GetAxis("Horizontal") * moveSpeed;
        float MoveJump = Input.GetAxis("Vertical") * jumpSpeed;

        Vector3 movement = new Vector3(0.0f, MoveJump, MoveHorz);

        rb.AddForce(movement * moveSpeed);
	}
}

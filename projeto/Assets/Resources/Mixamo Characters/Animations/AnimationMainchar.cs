using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMainchar : MonoBehaviour
{
    Animator playerAnim;
	Rigidbody playerRigid;
	public float w_speed, wb_speed, olw_speed, rn_speed, ro_speed;
	bool walking;
    Transform playerTrans;
	
    void Start(){
        playerAnim = GetComponent<Animator>();
        playerRigid = GetComponent<Rigidbody>();
        playerTrans = GetComponent<Transform>();
    }
	
	void FixedUpdate(){

        Vector3 velocity = Vector3.zero;
		if(Input.GetKey(KeyCode.W)){
			velocity = transform.forward * w_speed * Time.deltaTime;
		}
		if(Input.GetKey(KeyCode.S)){
			velocity = -transform.forward * wb_speed * Time.deltaTime;
		}

         playerRigid.velocity = velocity;
	}
	void Update(){

		if(Input.GetKeyDown(KeyCode.W)){
			playerAnim.SetTrigger("walk");
			playerAnim.ResetTrigger("idle");
			walking = true;
		}
		if(Input.GetKeyUp(KeyCode.W)){
			playerAnim.ResetTrigger("walk");
			playerAnim.SetTrigger("idle");
			walking = false;
		}
		if(Input.GetKeyDown(KeyCode.S)){
			playerAnim.SetTrigger("back");
			playerAnim.ResetTrigger("idle");
		}
		if(Input.GetKeyUp(KeyCode.S)){
			playerAnim.ResetTrigger("back");
			playerAnim.SetTrigger("idle");
		}
		if(Input.GetKey(KeyCode.A)){
			playerTrans.Rotate(0, -ro_speed * Time.deltaTime, 0);
		}
		if(Input.GetKey(KeyCode.D)){
			playerTrans.Rotate(0, ro_speed * Time.deltaTime, 0);
		}
    
		if(walking == true){				
			if(Input.GetKeyDown(KeyCode.LeftShift)){
				w_speed = w_speed + rn_speed;
				playerAnim.SetTrigger("run");
				playerAnim.ResetTrigger("walk");
			}
			if(Input.GetKeyUp(KeyCode.LeftShift)){
				w_speed = olw_speed;
				playerAnim.ResetTrigger("run");
				playerAnim.SetTrigger("walk");
			}
		}
	}
}

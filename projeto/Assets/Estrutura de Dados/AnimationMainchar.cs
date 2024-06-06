using UnityEngine;

public class AnimationMainchar : MonoBehaviour
{
    Animator playerAnim;
    CharacterController characterController;
    float walkSpeed, walkBackSpeed, originalWalkSpeed, runAddSpeed, rotationSpeed;
    public float jumpForce = 5f;
    public float gravity = 9.81f;
    float verticalSpeed = 0f; // variável para armazenar a velocidade vertical
    bool walking;
    bool isGrounded;
    bool wasFalling;
    bool justLand;
    bool jumping;
	bool running;

    public float groundCheckDistance = 1.0f;

    Transform playerTrans;

    void Start()
    {
        playerAnim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        playerTrans = GetComponent<Transform>();
        originalWalkSpeed = 2f;
        walkSpeed = originalWalkSpeed;
        walkBackSpeed = 1.5f;
        runAddSpeed = 3f;
        rotationSpeed = 100f;
    }

    void FixedUpdate(){
        Vector3 move = Vector3.zero;
        isGrounded = characterController.isGrounded;
		AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);

		if (Input.GetKey(KeyCode.W) && !stateInfo.IsName("Landing")){
			move += transform.forward * walkSpeed * Time.deltaTime;
				
		}if (Input.GetKey(KeyCode.S) && !stateInfo.IsName("Landing")){
			move += -transform.forward * walkBackSpeed * Time.deltaTime;
		}

        if (isGrounded){
			if (jumping){
				verticalSpeed = jumpForce; 
        
        	}else if (!jumping){
            verticalSpeed = -gravity * Time.deltaTime; // Mantém o personagem no chão
        	}
        }else{
            verticalSpeed -= gravity * Time.deltaTime; // Aplica gravidade
        }

        move.y = verticalSpeed * Time.deltaTime;
        characterController.Move(move);
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);

        if (jumping && stateInfo.IsName("Jump") && stateInfo.normalizedTime >= 1.0f)
        {
            playerAnim.ResetTrigger("jump");
            jumping = false;
        }

         if (Input.GetKey(KeyCode.A))
            {
                playerTrans.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
               playerTrans.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            }

		
				if (Input.GetKeyDown(KeyCode.Space)){
					playerAnim.SetTrigger("jump");
					jumping = true;
				}

				if (Input.GetKey(KeyCode.W))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        walkSpeed = originalWalkSpeed + runAddSpeed;
                        playerAnim.SetTrigger("run");
                        running = true;
                    }
                    else
                    {
                        walkSpeed = originalWalkSpeed;
                        playerAnim.SetTrigger("walk");
						playerAnim.ResetTrigger("run");
						running = false;
                    }
                    playerAnim.ResetTrigger("idle");
                    walking = true;
                }
                if (Input.GetKeyUp(KeyCode.W))
                {
                    playerAnim.ResetTrigger("walk");
                    walking = false;
                    if (running)
                    {
                        playerAnim.ResetTrigger("run");
                        running = false;
                    }
					playerAnim.SetTrigger("idle");
                }
				if (Input.GetKey(KeyCode.S))
				{
					playerAnim.SetTrigger("back");
					playerAnim.ResetTrigger("idle");
				}
				if (Input.GetKeyUp(KeyCode.S))
				{
					playerAnim.ResetTrigger("back");
					playerAnim.SetTrigger("idle");
				}

        if (isGrounded)
        {
            playerAnim.ResetTrigger("fall");

            if (justLand && stateInfo.IsName("Landing") && stateInfo.normalizedTime >= 1.0f)
            {
                playerAnim.ResetTrigger("land");
				playerAnim.SetTrigger("idle");
                justLand = false;
            }

            if (wasFalling)
            {
                playerAnim.SetTrigger("land");
                wasFalling = false;
                justLand = true;
            }


            if (walking == true)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    walkSpeed = walkSpeed + runAddSpeed;
                    playerAnim.SetTrigger("run");
                    playerAnim.ResetTrigger("walk");
					running = true;
                }
                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    walkSpeed = originalWalkSpeed;
                    playerAnim.SetTrigger("walk");
                    playerAnim.ResetTrigger("run");
					running = false;
                }
            }
        }
        else
        {
            if (!jumping)
            {
                playerAnim.SetTrigger("fall");
                wasFalling = true;
            }
        }

    }

    //função de debug, esfera fica verde caso esteja grounded e vermelho caso contrario!
    //void OnDrawGizmos()
    //{
        //if (characterController != null)
        //{
            //Gizmos.color = isGrounded ? Color.green : Color.red;
            //Gizmos.DrawSphere(characterController.bounds.center - new Vector3(0, characterController.bounds.extents.y, 0), 0.1f);
        //}
    //}
}

using UnityEngine;

public class AnimationMainchar : MonoBehaviour{
    //Classe responsável pelos movimentos e animações do Personagem Principal da Parte2 
    Animator playerAnim; //instância Animator do personagem
    CharacterController characterController; //instância CharacterController do personagem
    //variaveis que indicam as velocidades do personagem
    //são respetivamente, a de andar; a de andar para trás; a original (para efeitos de recuperação); a velocidade extra da corrida
    // e a de rotação
    float walkSpeed, walkBackSpeed, originalWalkSpeed, runAddSpeed, rotationSpeed; 
    //a variável que indica a força do salto do personagem
    public float jumpForce = 5f;
    //variavel de gravidade exercida no personagem
    public float gravity = 9.81f;
    //variavel que serve para aplicar a gravidade no personagem
    float verticalSpeed = 0f;
    //booleans indicativos de o personagem: andar, estar no chão, estar a cair, acabar de aterrar,saltar e correr (respetivamente)
    bool walking;
    bool isGrounded;
    bool wasFalling;
    bool justLand;
    bool jumping;
	bool running;
    //transformada do personagem
    Transform playerTrans;


    //função de Start() que faz as associações necessárias com valores escolhidos pelo grupo
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

    //função de FixedUpdate que trata de realizar os movimentos na transformada do personagem
    void FixedUpdate(){
        Vector3 move = Vector3.zero; //Vector3 que irá definir a distancia percorrida
        isGrounded = characterController.isGrounded;
		AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);

		if (Input.GetKey(KeyCode.W) && !stateInfo.IsName("Landing")){
			move += transform.forward * walkSpeed * Time.deltaTime;// faz o personagem andar para a frente
				
		}if (Input.GetKey(KeyCode.S) && !stateInfo.IsName("Landing")){
			move += -transform.forward * walkBackSpeed * Time.deltaTime;//faz o personagem andar para trás
		}

        if (isGrounded){
			if (jumping){
				verticalSpeed = jumpForce; // faz o personagem saltar
        
        	}else if (!jumping){
            verticalSpeed = -gravity * Time.deltaTime; // Mantém o personagem no chão
        	}
        }else{
            verticalSpeed -= gravity * Time.deltaTime; // Aplica gravidade
        }

        move.y = verticalSpeed * Time.deltaTime;
        characterController.Move(move); //aplica o Vector3 move ao personagem através do characterController
    }

    //função de Update que é responsável pelas animações do personagem 
    void Update()
    {
        AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0); //obtem o atual estado de animação

        if (jumping && stateInfo.IsName("Jump") && stateInfo.normalizedTime >= 1.0f)
        {//se o personagem estiver a saltar e a animação acabar faz reset do trigger correspondente e mete o boolean a false
            playerAnim.ResetTrigger("jump");
            jumping = false;
        }

         if (Input.GetKey(KeyCode.A)) //se for pressisonado o A
            {
                playerTrans.Rotate(0, -rotationSpeed * Time.deltaTime, 0); //ocorre uma rotação do personagem para a esquerda
            }
        if (Input.GetKey(KeyCode.D)) //se for precisonado o D
            {
               playerTrans.Rotate(0, rotationSpeed * Time.deltaTime, 0); //ocorre uma rotação do personagem para a direita
        }

		
		if (Input.GetKeyDown(KeyCode.Space)){ // se for pressionado o espaço o personagem entra na animação de jump e mete 
                //o boolean respetivo a true
					playerAnim.SetTrigger("jump");
					jumping = true;
		}

		if (Input.GetKey(KeyCode.W)) //se fo pressionado o W 
            {
            if (Input.GetKey(KeyCode.LeftShift)) //e o LeftShift ao mesmo tempo entra no estado running e a velocidade 
                    //do personagem é atualizada para refletir a velocidade nova
            {
                walkSpeed = originalWalkSpeed + runAddSpeed;
                playerAnim.SetTrigger("run");
                running = true;
            }
            else //se não estiver a ser pressionado o Left Shift assume-se que está a andar
            //então atualiza a velocidade para a original e acciona o estado de walk
            {
                walkSpeed = originalWalkSpeed;
                playerAnim.SetTrigger("walk");
				playerAnim.ResetTrigger("run");
				running = false;
            }//independentemente de estar walking ou running coloca o bool walking a true (porque mesmo assim esta em movimento)
                playerAnim.ResetTrigger("idle");
                walking = true;
        }

        if (Input.GetKeyUp(KeyCode.W)) // se o user largar a tecla W assume-se que quer parar então dá reset dos Triggers
                //de andar e correr para meter em idle (também são alterados os boolean respetivos)
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

		if (Input.GetKey(KeyCode.S)) // se o S for pressionado o personagem entra no estado back (que indica andar para trás)
		{
			playerAnim.SetTrigger("back");
			playerAnim.ResetTrigger("idle");
		}

		if (Input.GetKeyUp(KeyCode.S)) // se o S deixar de ser pressionado o personagem volta para idle
		{
		    playerAnim.ResetTrigger("back");
			playerAnim.SetTrigger("idle");
		}

        if (isGrounded) // verifica se o personagem está no chão
        {
            playerAnim.ResetTrigger("fall"); //se tiver faz reset de fall (animação de queda)

            if (justLand && stateInfo.IsName("Landing") && stateInfo.normalizedTime >= 1.0f)
            {//se acabou de aterrar e terminou a animação vai para idle
                playerAnim.ResetTrigger("land");
				playerAnim.SetTrigger("idle");
                justLand = false;
            }

            if (wasFalling)
            {//se estava a cair dá trigger da animação de aterrar
                playerAnim.SetTrigger("land");
                wasFalling = false;
                justLand = true;
            }


            if (walking == true)
            {//se estiver a andar verifica o estado da tecla LeftShift (de modo a garantir que caso esteja ainda a ser pressionada
            //a animação de correr continua)
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
        else //caso não esteja grounded
        {
            if (!jumping) //assume-se que caso não esteja num salto então está a cair
            {
                playerAnim.SetTrigger("fall");
                wasFalling = true;
            }
        }

    }

    //função de debug, adiciona uma esfera no editor que fica verde caso esteja grounded e vermelho caso contrario
    //void OnDrawGizmos()
    //{
        //if (characterController != null)
        //{
            //Gizmos.color = isGrounded ? Color.green : Color.red;
            //Gizmos.DrawSphere(characterController.bounds.center - new Vector3(0, characterController.bounds.extents.y, 0), 0.1f);
        //}
    //}
}

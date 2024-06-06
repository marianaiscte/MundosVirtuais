using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderController : MonoBehaviour
{
    //Instancia Animator do Defender
    private Animator animator;
    //Instancia Animator do Attacker
    private Animator animatorAttacker;
    //Tag para encontrar o Attacker
    public string attckerTag = "Attacker";
    //boolean referente ao Block 
    private bool isblocking = false;
    //boolean referente ao Dodge 
    private bool dodged = false;
    //Transformada do attacker
    private Transform attacker = null;


    void Start()
    {
        animator = GetComponent<Animator>(); // Obtém o componente Animator
        animator.SetTrigger("Idle");

        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }

        FindAttacker();//procura pelo attacker
        
    }
//função de update que trata das animações do defender
    void Update()
    {

        AnimatorStateInfo stateInfoDefender = animator.GetCurrentAnimatorStateInfo(0);// estado de animação do defender
        AnimatorStateInfo stateInfoAttacker = animatorAttacker.GetCurrentAnimatorStateInfo(0);// estado de animação do attacker

        if(isblocking && stateInfoDefender.normalizedTime >= 1.0f && stateInfoDefender.IsName("Block")){
            //verifica se o defender esta a defender e já acabou a animação de block
                animator.ResetTrigger("Block");
                animator.SetTrigger("Idle");
                isblocking = false;

        }

        if(dodged && stateInfoDefender.normalizedTime >= 1.0f && stateInfoDefender.IsName("Dodge Back")){
            //verifica se o defender deu dodge e ja terminou a animação para agora voltar à posiçao inicial
                animator.ResetTrigger("DodgeBack");
                animator.SetTrigger("DodgeFront");
                dodged = false;
        }

        if(stateInfoDefender.normalizedTime >= 1.0f && stateInfoDefender.IsName("Dodge front") && !stateInfoAttacker.IsName("attack")){
            //verifica se o defender ja voltou do dodge e terminou a animação para voltar a ficar idle.
            //só acontece se não for atacado no caminho de volta
            animator.ResetTrigger("DodgeFront");
            animator.SetTrigger("Idle");
        
        }

        
        if(stateInfoAttacker.IsName("attack") && !dodged && !isblocking && !stateInfoDefender.IsName("Block") && !stateInfoDefender.IsName("Dodge Back")){
            //parte que determina a próxima jogada do defender,
            //caso o attacker esteja a attack e o defender não esteja a dar dodge, ou a bloquear (incluindo o término dessas animações)
            
            //morre se estiver a voltar do dodge
            if(stateInfoDefender.IsName("Dodge front")){
                animator.ResetTrigger("DodgeFront");
                animator.SetTrigger("Die");
            }else{
                //ou calcula a probabilidade de sobreviver
                float randomValue = Random.Range(0f, 1f);
                //se o valor for menor que 0.3 -> faz um dodge
                //se for entre 0.3 e 0.9 irá fazer block
                //se for maior que 0.9 o defender morre
                Debug.Log(randomValue);
                if(randomValue <= 0.3){
                    animator.ResetTrigger("Idle");
                    animator.SetTrigger("DodgeBack");
                    dodged = true;
                }else if (randomValue < 0.9){
                    animator.ResetTrigger("Idle");
                    animator.SetTrigger("Block");
                    isblocking = true;
                }else{
                    animator.SetTrigger("Die");
                }
            }

        }
           
    }

    //Função que procura o attacker
       void FindAttacker()
    {
        GameObject attackerObject = GameObject.FindWithTag(attckerTag);
        if (attackerObject != null){
            attacker = attackerObject.transform;
            animatorAttacker = attackerObject.GetComponent<Animator>();
        }
    }
}

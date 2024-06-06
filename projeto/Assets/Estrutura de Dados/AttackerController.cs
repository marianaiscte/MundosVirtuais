using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerController : MonoBehaviour
{
    //Instancia Animator do attacker
    private Animator animatorAttacker;
    //Instancia Animator do defender
    private Animator animatorDefender;
    //Tag para encontrar o Defender
    public string defenderTag = "Defender"; 
    //probabilidade do attacker atacar em cada update 
    public float attackProbability = 0.01f;  
    //boolean referente ao ataque
    public bool isAttacking = false;
    //boolean referente à celebraçao
    private bool won = false;
    //boolean referente à primeira (e unica) vitória
    private bool firstWon = false;
    //tempo do último ataque
    private float lastAttackTime = 0.0f;
    //coolDown do ataque
    public float attackCooldown = 2.0f;   
    //transformada do defender
    private Transform defender = null;

    void Start()
    {
        animatorAttacker = GetComponent<Animator>(); // Obtém o componente Animator
        if (animatorAttacker == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }

        FindDefender();//procura pelo defender
        lastAttackTime = Time.time;// guarda o tempo de execução para começar o padrão de ataques
    }

//função de update que trata das animações do attacker
    void Update()
    {
        AnimatorStateInfo stateInfoA = animatorAttacker.GetCurrentAnimatorStateInfo(0);// estado de animação do attacker
        AnimatorStateInfo stateInfoD = animatorDefender.GetCurrentAnimatorStateInfo(0);// estado de animação do defender

        if (stateInfoD.IsName("die")){ //verifica se o defender está morto
                won = true;
        }

        if(won && !firstWon && stateInfoA.IsName("Victory") &&  stateInfoA.normalizedTime >= 1.0f){ //verifica
        //se o attacker ja celebrou a morte do defender
            animatorAttacker.ResetTrigger("Victory");
            animatorAttacker.SetTrigger("Idle");
            won = false;
            firstWon = true;

        }

        
        if (won && !firstWon) //verifica se é a primeira (no caso unica) vitória do attacker
        {
            animatorAttacker.ResetTrigger("Attacking");
            animatorAttacker.ResetTrigger("Idle");
            animatorAttacker.SetTrigger("Victory");
        }


        if(isAttacking && stateInfoA.normalizedTime >= 1.0f && stateInfoA.IsName("attack")){ //verifica se
        //o attacker ja acabou a animação de attacking para voltar a idle
            animatorAttacker.ResetTrigger("Attacking");
            animatorAttacker.SetTrigger("Idle");
            lastAttackTime = Time.time;            
            isAttacking = false;

        }

        if (!isAttacking && Time.time >= lastAttackTime + attackCooldown && !stateInfoD.IsName("die")){ // verifica se
        //não está a ocorrer um ataque, se o coldown de ataque já terminou e se o defender já morreu 
            float randomValue = Random.Range(0f, 1f);//probabilidade que define a próxima ação (idle ou ataque)
            if (randomValue <= attackProbability)
            {//caso seja ataque
                //Debug.Log(randomValue);
                animatorAttacker.ResetTrigger("Idle");
                animatorAttacker.SetTrigger("Attacking");
                isAttacking = true;

            }
            else
            {//caso seja idle
                animatorAttacker.ResetTrigger("Attacking");
                animatorAttacker.SetTrigger("Idle");
            }
        }

    }

    //Função que procura defender
    void FindDefender()
    {
        GameObject defenderObject = GameObject.FindWithTag(defenderTag);
        if (defenderObject != null){
            defender = defenderObject.transform;
            animatorDefender = defenderObject.GetComponent<Animator>();
            
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerController : MonoBehaviour
{
    private Animator animatorAttacker;

    private Animator animatorDefender;
    public string defenderTag = "Defender"; 
    public float attackProbability = 0.01f;  
    public bool isAttacking = false;

    private bool won = false;

    private float lastAttackTime = 0.0f;

    public float attackCooldown = 2.0f;   

    private Transform defender = null;

    void Start()
    {
        animatorAttacker = GetComponent<Animator>(); // Obtém o componente Animator
        if (animatorAttacker == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }

        FindDefender();
        lastAttackTime = Time.time;
    }

    void Update()
    {
        AnimatorStateInfo stateInfoA = animatorAttacker.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo stateInfoD = animatorDefender.GetCurrentAnimatorStateInfo(0);

        if (stateInfoD.IsName("die")){
                won = true;
        }

        
        if (won)
        {
            animatorAttacker.ResetTrigger("Attacking");
            animatorAttacker.ResetTrigger("Idle");
            animatorAttacker.SetTrigger("Victory");
        }


        if(isAttacking && stateInfoA.normalizedTime >= 1.0f && stateInfoA.IsName("attack")){
            lastAttackTime = Time.time;
            animatorAttacker.ResetTrigger("Attacking");
            animatorAttacker.SetTrigger("Idle");
            AttackOccurred();
            isAttacking = false;

        }

        if (!isAttacking && Time.time >= lastAttackTime + attackCooldown){
            float randomValue = Random.Range(0f, 1f);
            if (randomValue <= attackProbability)
            {
                //Debug.Log(randomValue);
                animatorAttacker.ResetTrigger("Idle");
                animatorAttacker.SetTrigger("Attacking");
                isAttacking = true;

            }
            else
            {
                animatorAttacker.ResetTrigger("Attacking");
                animatorAttacker.SetTrigger("Idle");
            }
        }

    }

    void FindDefender()
    {
        GameObject defenderObject = GameObject.FindWithTag(defenderTag);
        if (defenderObject != null){
            defender = defenderObject.transform;
            animatorDefender = defenderObject.GetComponent<Animator>();
            
        }
    }

    // Adicione este método para atualizar o lastAttackTime quando um ataque ocorrer
    public void AttackOccurred()
    {
        lastAttackTime = Time.time;
        //Debug.Log(lastAttackTime);
    }
}

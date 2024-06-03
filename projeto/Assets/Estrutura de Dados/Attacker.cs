using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackerController : MonoBehaviour
{
    private Animator animator;
    public string defenderTag = "Defender"; 
    public float attackProbability = 0.01f;  
    private bool isAttacking = false;
    private float lastAttackTime = 0.0f;

    public float attackCooldown = 10.0f;   

    private Transform defender = null;

    void Start()
    {
        animator = GetComponent<Animator>(); // Obtém o componente Animator
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }

        FindDefender();
        lastAttackTime = Time.time;
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if(isAttacking && stateInfo.normalizedTime >= 1.0f && stateInfo.IsName("attack")){
            lastAttackTime = Time.time;
            animator.ResetTrigger("Attacking");
            animator.SetTrigger("Idle");
            AttackOccurred();
            isAttacking = false;

        }

        if (!isAttacking && Time.time >= lastAttackTime + attackCooldown){
            float randomValue = Random.Range(0f, 1f);
            if (randomValue <= attackProbability)
            {
                Debug.Log(randomValue);
                animator.ResetTrigger("Idle");
                animator.SetTrigger("Attacking");
                isAttacking = true;

            }
            else
            {
                animator.ResetTrigger("Attacking");
                animator.SetTrigger("Idle");
            }
        }

        if (defender == null || !defender.gameObject.activeSelf)
        {
            animator.ResetTrigger("Attacking");
            animator.ResetTrigger("Idle");
            animator.SetTrigger("Victory");
        }
    }

    void FindDefender()
    {
        GameObject defenderObject = GameObject.FindWithTag(defenderTag);
        if (defenderObject != null){
            defender = defenderObject.transform;
        }
    }

    // Adicione este método para atualizar o lastAttackTime quando um ataque ocorrer
    public void AttackOccurred()
    {
        lastAttackTime = Time.time;
        //Debug.Log(lastAttackTime);
    }
}

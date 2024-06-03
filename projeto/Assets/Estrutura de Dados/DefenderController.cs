using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenderController : MonoBehaviour
{

    private Animator animator;

    private Animator animatorAttacker;

    public string attckerTag = "Attacker"; 
    private bool isblocking = false;
    private bool dodged = false;
    private Transform attacker = null;

    private GameObject attackerObject;

    void Start()
    {
        animator = GetComponent<Animator>(); 
        animator.SetTrigger("Idle");

        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }

        FindAttacker();
        
    }

    void Update()
    {

        AnimatorStateInfo stateInfoDefender = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo stateInfoAttacker = animatorAttacker.GetCurrentAnimatorStateInfo(0);

        if(isblocking && stateInfoDefender.normalizedTime >= 1.0f && stateInfoDefender.IsName("Block")){
                animator.ResetTrigger("Block");
                animator.SetTrigger("Idle");
                isblocking = false;

        }

        if(dodged && stateInfoDefender.normalizedTime >= 1.0f && stateInfoDefender.IsName("Dodge Back")){
                animator.ResetTrigger("DodgeBack");
                animator.SetTrigger("DodgeFront");
                dodged = false;
        }

        if(stateInfoDefender.normalizedTime >= 1.0f && stateInfoDefender.IsName("Dodge front")){
            animator.ResetTrigger("DodgeFront");
            animator.SetTrigger("Idle");
        
        }

        
        if(stateInfoAttacker.IsName("attack") && !dodged && !isblocking && !stateInfoDefender.IsName("Block") && !stateInfoDefender.IsName("Dodge Back")){
            
            if(stateInfoDefender.IsName("Dodge front")){
                animator.SetTrigger("Die");
            }
            
            float randomValue = Random.Range(0f, 1f);
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

       void FindAttacker()
    {
        GameObject attackerObject = GameObject.FindWithTag(attckerTag);
        if (attackerObject != null){
            attacker = attackerObject.transform;
            animatorAttacker = attackerObject.GetComponent<Animator>();
        }
    }
}

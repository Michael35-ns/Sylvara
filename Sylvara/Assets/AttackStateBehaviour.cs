using UnityEngine;
using System.Collections;

public class AttackStateBehaviour : StateMachineBehaviour
{
    private bool canQueueNextAttack = false;
    private float comboResetTime = 1f;
    private float cooldownTime = 1f;
    private float lastAttackTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<PlayerController>().SetCanMove(false);

        animator.SetBool("PuedoDarClick", false);
        canQueueNextAttack = false;
        lastAttackTime = Time.time;

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!canQueueNextAttack && stateInfo.normalizedTime >= 0.8f)
        {
            animator.SetBool("PuedoDarClick", true);
            canQueueNextAttack = true;
            lastAttackTime = Time.time;
        }

        if (Time.time - lastAttackTime > comboResetTime)
        {
            animator.SetInteger("Attack", 0);
            animator.SetBool("PuedoDarClick", true);
            animator.Play("Idle");
            animator.GetComponent<PlayerController>().SetCanMove(true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetInteger("Attack") >= 3)
        {
            animator.SetInteger("Attack", 0);
            animator.SetBool("PuedoDarClick", false);
            animator.GetComponent<PlayerController>().StartCoroutine(EnableAttackAfterCooldown(animator));
        }

        animator.GetComponent<PlayerController>().SetCanMove(true);
    }

    private IEnumerator EnableAttackAfterCooldown(Animator animator)
    {
        yield return new WaitForSeconds(cooldownTime);
        animator.SetBool("PuedoDarClick", true);
        Debug.Log("🔥 Cooldown finalizado, puede volver a atacar.");
    }
}

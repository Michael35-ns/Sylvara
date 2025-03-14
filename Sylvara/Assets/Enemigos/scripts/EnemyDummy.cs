using UnityEngine;

public class EnemyDummy : MonoBehaviour
{
    private HealthSystem healthSystem;
    private Animator animator;

    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        healthSystem.TakeDamage(damage);

        // 🔹 Mostrar animación de golpe
        animator.SetTrigger("Hurt");

        if (healthSystem.GetHealth() <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("Death");
        Debug.Log($"💀 {gameObject.name} ha muerto.");

        // 🔹 Desactivarlo tras la animación de muerte
        StartCoroutine(DisableAfterDeath());
    }

    private System.Collections.IEnumerator DisableAfterDeath()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 0.5f);
        gameObject.SetActive(false);
    }
}

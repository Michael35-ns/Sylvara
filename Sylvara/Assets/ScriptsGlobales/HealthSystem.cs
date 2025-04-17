using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public bool isDead = false;

    public float defense = 5f;
    public float invulnerabilityTime = 1.5f;

    private bool isInvulnerable = false;
    private Animator animator;
    private CharacterController controller;

    public delegate void DeathEvent();
    public event DeathEvent OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        // StartCoroutine(RegenerateHealth());
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isInvulnerable) return;

        float damageTaken = Mathf.Max(damage - defense, 1);
        currentHealth -= damageTaken;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"⚠️ {gameObject.name} recibió {damageTaken} de daño. Vida restante: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (animator != null)
                animator.SetTrigger("Hurt");

            StartCoroutine(Invulnerability());
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetBool("Dead", true);

        if (controller != null)
            controller.enabled = false;

        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
            playerController.enabled = false;

        if (OnDeath != null)  // Corrección aquí
            OnDeath.Invoke();

        StartCoroutine(DisableAfterDeath());
    }

    private IEnumerator DisableAfterDeath()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 1f);
        gameObject.SetActive(false);
    }

    private IEnumerator Invulnerability()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public float GetHealth() => currentHealth;
}


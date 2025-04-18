using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public bool isDead = false;

    public float defense = 5f;
    public float invulnerabilityTime = 1.5f;

    public PlayerUI playerUI;

    private bool isInvulnerable = false;
    private Animator animator;
    private CharacterController controller;

    private Coroutine invulRoutine;

    public delegate void DeathDelegate();
    public event DeathDelegate OnDeath;

    private Coroutine deathRoutine;


    public enum CharacterType
    {
        Player,
        Enemy,
        Boss
    }

    public CharacterType characterType = CharacterType.Enemy;


    void Start()
    {
        currentHealth = maxHealth;
        if (playerUI != null)
            playerUI.UpdateHealth(currentHealth, maxHealth);

        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    public void SetInvulnerable(bool value)
    {
        isInvulnerable = value;
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isInvulnerable) return;

        float damageTaken = Mathf.Max(damage - defense, 1);
        currentHealth -= damageTaken;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (playerUI != null)
            playerUI.UpdateHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hurt");

            if (invulRoutine != null)
                StopCoroutine(invulRoutine);

            invulRoutine = StartCoroutine(Invulnerability());
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        OnDeath?.Invoke();

        animator.SetBool("Dead", true);

        if (controller != null)
            controller.enabled = false;

        if (characterType == CharacterType.Player)
        {
            GetComponent<PlayerController>().enabled = false;
            FindObjectOfType<GameUIManager>()?.ShowDefeatScreen();
        }
        else if (characterType == CharacterType.Boss)
        {
            FindObjectOfType<GameUIManager>()?.ShowVictoryScreen();
        }

        if (deathRoutine != null)
            StopCoroutine(deathRoutine);

        deathRoutine = StartCoroutine(DisableAfterDeath());
    }


    private IEnumerator DisableAfterDeath()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 1f);

        DisableComponents();

        deathRoutine = null;
    }

    public void ReactivateComponents()
    {
        if (deathRoutine != null)
        {
            StopCoroutine(deathRoutine);
            deathRoutine = null;
        }

        var pc = GetComponent<PlayerController>();
        if (pc != null) pc.enabled = true;

        var cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = true;

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        isDead = false;

        if (animator != null)
        {
            animator.SetBool("Dead", false);
            animator.Rebind();
            animator.SetBool("PuedoDarClick", true);
        }
    }


    private void DisableComponents()
    {
        var pc = GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;

        var cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }


    private IEnumerator Invulnerability()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        if (playerUI != null)
            playerUI.UpdateHealth(currentHealth, maxHealth);
    }

    public float GetHealth() => currentHealth;
}

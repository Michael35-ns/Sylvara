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
        if (playerUI != null)
            playerUI.UpdateHealth(currentHealth, maxHealth);
    }

    public float GetHealth() => currentHealth;
}

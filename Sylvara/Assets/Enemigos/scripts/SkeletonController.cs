using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonController : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 3f;
    public float moveSpeed = 2f;
    public float attackDamage = 20f;  // Daño del ataque
    public float attackCooldown = 4f;  // Enfriamiento entre ataques

    private Animator animator;
    private NavMeshAgent agent;
    private bool isAngry = false;
    private bool isSensing = false;
    private bool isAttacking = false;
    private bool canAttack = true;

    private HealthSystem healthSystem;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        healthSystem = GetComponent<HealthSystem>();

        if (animator == null) Debug.LogError("Animator no asignado en: " + gameObject.name);
        if (agent == null) Debug.LogError("NavMeshAgent no asignado en: " + gameObject.name);
        if (healthSystem == null) Debug.LogError("HealthSystem no asignado en: " + gameObject.name);

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("No se encontró GameObject con tag 'Player'. Asigna manualmente en el Inspector.");
        }

        if (agent != null)
            agent.speed = moveSpeed;

        if (healthSystem != null)
            healthSystem.OnDeath += OnDeath;  // Suscripción al evento de muerte
    }

    void Update()
    {
        if (healthSystem == null || healthSystem.isDead) return;
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!isAngry && distanceToPlayer <= detectionRange)
        {
            isAngry = true;
            if (animator != null)
            {
                animator.SetTrigger("Combat_Stance");
                StartCoroutine(EnterBattleIdle());
            }
        }

        if (isAngry)
        {
            if (distanceToPlayer > attackRange)
            {
                if (!isSensing && !isAttacking)
                {
                    SensePlayer();
                }
                ChasePlayer();
            }
            else if (!isAttacking && canAttack)
            {
                AttackPlayer();
            }
        }
    }

    IEnumerator EnterBattleIdle()
    {
        yield return new WaitForSeconds(1f);
    }

    void ChasePlayer()
    {
        if (agent == null || player == null) return;

        if (agent.isStopped) agent.isStopped = false;
        agent.SetDestination(player.position);

        float distance = Vector3.Distance(transform.position, player.position);

        if (animator != null)
        {
            bool running = distance > detectionRange * 1.5f;
            animator.SetBool("Run", running);
            animator.SetBool("Idle", !running);
        }
    }

    void AttackPlayer()
    {
        isAttacking = true;
        canAttack = false;

        if (agent != null) agent.isStopped = true;

        if (animator != null)
        {
            int attackType = Random.Range(0, 2);
            animator.SetTrigger(attackType == 0 ? "Strike_1" : "Strike_2");
        }

        StartCoroutine(DealDamageWithDelay(0.5f));  // Retraso para aplicar el daño
        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator DealDamageWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (player != null)
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();

            if (playerHealth != null && !playerHealth.isDead)
            {
                playerHealth.TakeDamage(attackDamage);  // Aplica el daño al jugador
                Debug.Log($"💀 El esqueleto atacó al jugador y le hizo {attackDamage} de daño.");
            }
        }
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        canAttack = true;
    }

    void SensePlayer()
    {
        isSensing = true;
        StartCoroutine(SenseRoutine());
    }

    IEnumerator SenseRoutine()
    {
        if (animator != null)
        {
            animator.SetTrigger("Combat_Stance");
            yield return new WaitForSeconds(1f);

            animator.SetTrigger("Combat_Stance");
            yield return new WaitForSeconds(1.5f);
        }

        isSensing = false;
    }

    public void TakeDamage(float damage)
    {
        if (healthSystem == null || healthSystem.isDead) return;

        healthSystem.TakeDamage(damage);

        if (!healthSystem.isDead)
        {
            if (animator != null)
                animator.SetTrigger("Combat_Stance");
        }
    }

    private void OnDeath()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        if (animator != null)
        {
            animator.SetTrigger("Dead_1");
        }

        Destroy(gameObject, 3f);
        yield return null;
    }
}

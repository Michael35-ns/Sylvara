using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MushroomController : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 3f;
    public float moveSpeed = 2f;

    private Animator animator;
    private NavMeshAgent agent;
    private bool isAngry = false;
    private bool isSensing = false;
    private bool isAttacking = false;

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
            healthSystem.OnDeath += OnDeath;  // suscripción al evento
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
                animator.SetTrigger("Mushroom_SenseSomethir");
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
            else if (!isAttacking)
            {
                Debug.Log("En rango de ataque, iniciando AttackPlayer()");
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
            animator.SetBool("isRunning", running);
            animator.SetBool("Mushroom_walkFWDAngry", !running);
        }
    }

    void AttackPlayer()
    {
        isAttacking = true;
        if (agent != null) agent.isStopped = true;

        if (animator != null)
        {
            int attackType = Random.Range(0, 2);
            Debug.Log("Disparando animación de ataque: " + (attackType == 0 ? "Mushroom_Attack01Angry" : "Mushroom_Attack02Angry"));

            animator.SetTrigger(attackType == 0 ? "Mushroom_Attack01Angry" : "Mushroom_Attack02Angry");
        }

        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(2f);
        isAttacking = false;
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
            animator.SetTrigger("Mushroom_SenseSomethir");
            yield return new WaitForSeconds(1f);

            animator.SetTrigger("Mushroom_Taunting");
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
                animator.SetTrigger("Mushroom_GetHit");
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
            animator.SetTrigger("Mushroom_Dizzy");
            yield return new WaitForSeconds(1f);
            animator.SetTrigger("Mushroom_Die");
        }

        Destroy(gameObject, 3f);
    }
}


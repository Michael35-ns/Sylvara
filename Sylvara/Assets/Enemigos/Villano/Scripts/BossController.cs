using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float safeDistance = 8f;
    public float teleportDistance = 6f;

    private Transform player;
    private CharacterController controller;
    private Animator animator;
    private HealthSystem healthSystem;

    private bool isCasting = false;
    private bool isAttacking = false;
    private float attackCooldownTimer;

    [Header("Ataques del Jefe")]
    public DashKick dashKick;
    public FireballAttack fireballAttack;
    public NukeAttack nukeAttack;
    public DeathRayAttack deathRayAttack;
    public BulletHellAttack bulletHellAttack;

    private bool usedDeathRay = false;
    private bool isPhaseTwo = false;

    [Header("Efectos del Jefe")]
    public GameObject teleportPuffEffect;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        attackCooldownTimer = Random.Range(4f, 7f);
    }

    void Update()
    {
        if (player == null) return;

        attackCooldownTimer -= Time.deltaTime;

        // Entrar a fase 2 si tiene baja vida
        float healthRatio = healthSystem.GetHealth() / healthSystem.maxHealth;
        bool lowHealthPhase = healthRatio <= 0.3f;

        if (!isPhaseTwo && lowHealthPhase)
        {
            isPhaseTwo = true;
            walkSpeed *= 1.5f;
            runSpeed *= 1.5f;
        }

        if (isAttacking || dashKick.IsDashing() || nukeAttack.IsCasting() || deathRayAttack.IsCasting())
        {
            SetAnim("Idle");
            return;
        }

        if (attackCooldownTimer <= 0f)
        {
            isAttacking = true;

            // ⚡ Lanza DeathRay por única vez si entra a fase 2
            if (lowHealthPhase && !usedDeathRay)
            {
                usedDeathRay = true;
                SetCasting(true);
                deathRayAttack.StartDeathRay(() =>
                {
                    isAttacking = false;
                    SetCasting(false);
                    attackCooldownTimer = Random.Range(4f, 7f);
                });
                return;
            }

            // 🎲 Elegimos ataque aleatorio
            int random = Random.Range(0, 100);

            if (!isPhaseTwo)
            {
                // Fase 1
                if (random < 50)
                {
                    fireballAttack.CastFireball(FinishAttack);
                }
                else if (random < 75)
                {
                    dashKick.StartDash(FinishAttack);
                }
                else if (random < 90)
                {
                    SetCasting(true);
                    nukeAttack.StartNuke(FinishAttackWithCastReset);
                }
                else
                {
                    bulletHellAttack.StartBulletHell(false, FinishAttackWithCastReset);
                    SetCasting(true);
                }
            }
            else
            {
                // Fase 2: patrón más agresivo
                if (random < 25)
                {
                    dashKick.StartDash(FinishAttack);
                }
                else if (random < 50)
                {
                    SetCasting(true);
                    nukeAttack.StartNuke(FinishAttackWithCastReset);
                }
                else if (random < 75)
                {
                    bulletHellAttack.StartBulletHell(true, FinishAttackWithCastReset);
                    SetCasting(true);
                }
                else if (random < 90)
                {
                    SetCasting(true);
                    deathRayAttack.StartDeathRay(FinishAttackWithCastReset);
                }
                else
                {
                    fireballAttack.CastFireball(FinishAttack);
                }
            }

            return;
        }

        UpdateMovementAndAnimation();
    }

    void FinishAttack()
    {
        isAttacking = false;
        attackCooldownTimer = isPhaseTwo ? Random.Range(2f, 4f) : Random.Range(4f, 7f);
    }

    void FinishAttackWithCastReset()
    {
        isAttacking = false;
        SetCasting(false);
        attackCooldownTimer = isPhaseTwo ? Random.Range(2f, 4f) : Random.Range(4f, 7f);
    }

    public void SetCasting(bool value)
    {
        isCasting = value;
    }

    void UpdateMovementAndAnimation()
    {
        if (isCasting)
        {
            SetAnim("Idle");
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);
        Vector3 direction = Vector3.zero;

        const float tooCloseDistance = 3f;
        const float chaseDistance = 15f;
        const float buffer = 0.5f;

        if (distance < tooCloseDistance)
        {
            TeleportAwayFromPlayer();
            SetAnim("Idle");
            return;
        }
        else if (distance > chaseDistance)
        {
            direction = (player.position - transform.position).normalized;
            Move(direction, runSpeed);
            SetAnim("Running");
        }
        else if (distance > safeDistance + buffer)
        {
            direction = (player.position - transform.position).normalized;
            Move(direction, walkSpeed);
            SetAnim("Walking");
        }
        else if (distance < safeDistance - buffer)
        {
            direction = (transform.position - player.position).normalized;
            Move(direction, walkSpeed);

            float angle = Vector3.Angle(transform.forward, direction);
            if (angle > 135f)
                SetAnim("WalkingBackwards");
            else
                SetAnim("Walking");
        }
        else
        {
            SetAnim("Idle");
        }

        RotateTowards(player.position);
    }

    void Move(Vector3 direction, float speed)
    {
        if (!controller.enabled) return;
        controller.Move(direction * speed * Time.deltaTime);
    }

    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 lookDirection = targetPosition - transform.position;
        lookDirection.y = 0;

        if (lookDirection.magnitude > 0.1f)
        {
            Quaternion rot = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
        }
    }

    void TeleportAwayFromPlayer()
    {
        if (isCasting) return;

        float angle = Random.Range(0f, 360f);
        Vector3 randomDirection = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;

        Vector3 newPosition = player.position + randomDirection * teleportDistance;
        newPosition.y = transform.position.y;

        if (teleportPuffEffect != null)
            Instantiate(teleportPuffEffect, transform.position, Quaternion.identity);

        controller.enabled = false;
        transform.position = newPosition;
        controller.enabled = true;

        if (teleportPuffEffect != null)
            Instantiate(teleportPuffEffect, transform.position, Quaternion.identity);

        RotateTowards(player.position);
    }

    void SetAnim(string state)
    {
        animator.SetBool("Walking", state == "Walking");
        animator.SetBool("Running", state == "Running");
        animator.SetBool("WalkingBackwards", state == "WalkingBackwards");
    }
}

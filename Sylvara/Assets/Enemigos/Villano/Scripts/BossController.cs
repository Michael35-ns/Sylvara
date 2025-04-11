using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float safeDistance = 8f;
    public float teleportDistance = 6f;

    [Header("Ataques del Jefe")]
    public DashKick dashKick;
    public FireballAttack fireballAttack;
    public NukeAttack nukeAttack;
    public DeathRayAttack deathRayAttack;
    public BulletHellAttack bulletHellAttack;

    [Header("Efectos")]
    public GameObject teleportPuffEffect;

    private Transform player;
    private CharacterController controller;
    private Animator animator;
    private HealthSystem healthSystem;
    [SerializeField] private BossHealthBar bossHealthBar;


    private bool isCasting = false;
    private bool isAttacking = false;
    private bool usedDeathRay = false;
    private bool isPhaseTwo = false;

    private float attackCooldownTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();
        attackCooldownTimer = Random.Range(4f, 7f);
        bossHealthBar.SetBoss(healthSystem);
    }

    void Update()
    {
        if (player == null) return;

        attackCooldownTimer -= Time.deltaTime;
        CheckPhase();

        if (isAttacking || dashKick.IsDashing() || nukeAttack.IsCasting() || deathRayAttack.IsCasting())
        {
            SetAnim("Idle");
            return;
        }

        if (attackCooldownTimer <= 0f)
        {
            TryAttack();
            return;
        }

        UpdateMovementAndAnimation();
    }

    void CheckPhase()
    {
        float healthRatio = healthSystem.GetHealth() / healthSystem.maxHealth;
        bool lowHealth = healthRatio <= 0.45f;

        if (!isPhaseTwo && lowHealth)
        {
            isPhaseTwo = true;
            walkSpeed *= 1.5f;
            runSpeed *= 1.5f;
        }
    }

    void TryAttack()
    {
        isAttacking = true;

        if (isPhaseTwo && !usedDeathRay)
        {
            usedDeathRay = true;
            SetCasting(true);
            deathRayAttack.StartDeathRay(() =>
            {
                FinishAttackWithCastReset();
            });
            return;
        }

        float rng = Random.Range(0f, 100f);

        if (!isPhaseTwo)
        {
            if (rng < 50f)
                fireballAttack.CastFireball(FinishAttack);
            else if (rng < 75f)
                dashKick.StartDash(FinishAttack);
            else if (rng < 95f)
            {
                SetCasting(true);
                nukeAttack.StartNuke(FinishAttackWithCastReset);
            }
            else
            {
                SetCasting(true);
                bulletHellAttack.StartBulletHell(false, FinishAttackWithCastReset);
            }
        }
        else
        {
            float fireball = 15f;   // Disminuido en 25%
            float dash = 25f;
            float nuke = 25f;
            float bullet = 20f;
            float deathray = 15f;

            if (rng < dash)
                dashKick.StartDash(FinishAttack);
            else if (rng < dash + nuke)
            {
                SetCasting(true);
                nukeAttack.StartNuke(FinishAttackWithCastReset);
            }
            else if (rng < dash + nuke + bullet)
            {
                SetCasting(true);
                bulletHellAttack.StartBulletHell(true, FinishAttackWithCastReset);
            }
            else if (rng < dash + nuke + bullet + deathray)
            {
                SetCasting(true);
                deathRayAttack.StartDeathRay(FinishAttackWithCastReset);
            }
            else
                fireballAttack.CastFireball(FinishAttack);
        }
    }

    void FinishAttack()
    {
        isAttacking = false;
        attackCooldownTimer = isPhaseTwo ? Random.Range(1f, 3f) : Random.Range(4f, 7f);
    }

    void FinishAttackWithCastReset()
    {
        isAttacking = false;
        SetCasting(false);
        attackCooldownTimer = isPhaseTwo ? Random.Range(1f, 3f) : Random.Range(4f, 7f);
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

        const float tooClose = 3f;
        const float chase = 15f;
        const float buffer = 0.5f;

        if (distance < tooClose)
        {
            TeleportAwayFromPlayer();
            SetAnim("Idle");
            return;
        }
        else if (distance > chase)
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
            SetAnim(angle > 135f ? "WalkingBackwards" : "Walking");
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
        Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        Vector3 newPos = player.position + direction * teleportDistance;
        newPos.y = transform.position.y;

        if (teleportPuffEffect != null)
            Instantiate(teleportPuffEffect, transform.position, Quaternion.identity);

        controller.enabled = false;
        transform.position = newPos;
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

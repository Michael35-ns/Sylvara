using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float moveSpeed = 2f;
    public float runSpeed = 4f;
    public float gravity = 9.8f;
    public float attackDamage = 20f;

    public Attack attack;

    private Vector3 moveDirection;
    private CharacterController controller;
    public Animator animator;
    private bool canMove = true;

    public bool tieneEspada = false;
    public GameObject espadaPrefab;
    public Transform puntoEspada;
    private GameObject espadaActual;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        animator.SetBool("PuedoDarClick", true);
    }

    void Update()
    {
        HandleMovement();
        HandleAttack();
        ApplyGravity();
    }

    // ---------------- MOVIMIENTO ----------------
    public void SetCanMove(bool value)
    {
        if (!value || animator.GetInteger("Attack") == 0)
        {
            canMove = value;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyDummy enemy = other.GetComponent<EnemyDummy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                Debug.Log($"⚔️ Golpeaste a {other.name} e hiciste {attackDamage} de daño.");
            }
        }
    }

    void HandleMovement()
    {
        if (!canMove) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(horizontal, 0, vertical);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        if (move.magnitude > 0.1f)
        {
            move.Normalize();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(move), Time.deltaTime * 10f);
        }

        animator.SetBool("TieneEspada", tieneEspada);
        animator.SetBool("Running", isRunning && move.magnitude > 0.1f);
        animator.SetFloat("Speed", move.magnitude * currentSpeed);

        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    // ---------------- ATAQUE ----------------
    void HandleAttack()
    {
        if (!tieneEspada) return;

        if (!animator.GetBool("PuedoDarClick"))
        {
            animator.SetBool("AtaqueEnCola", true);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {

            SetCanMove(false);
            animator.SetFloat("Speed", 0);
            animator.SetBool("Running", false);
            attack.SetOwner(gameObject);

            int currentAttack = animator.GetInteger("Attack");

            if (currentAttack == 0)
            {
                animator.SetInteger("Attack", 1);
                attack.PerformAttack();
            }
            else if (currentAttack == 1)
            {
                animator.SetInteger("Attack", 2);
                attack.PerformAttack();
            }
            else if (currentAttack == 2)
            {
                animator.SetInteger("Attack", 3);
                attack.PerformAttack();
            }

            animator.SetBool("PuedoDarClick", false);
        }
    }

    // ---------------- GRAVEDAD ----------------
    void ApplyGravity()
    {
        if (!controller.enabled) return;

        if (!controller.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else
        {
            moveDirection.y = -0.1f;
        }
        controller.Move(moveDirection * Time.deltaTime);
    }

    // ---------------- OBTENER ESPADA ----------------
    public void ObtenerEspada()
    {
        if (tieneEspada) return;

        tieneEspada = true;
        animator.SetBool("TieneEspada", true);

        if (espadaPrefab != null && puntoEspada != null)
        {
            espadaActual = Instantiate(espadaPrefab, puntoEspada);
            espadaActual.transform.localPosition = Vector3.zero;
            espadaActual.transform.localRotation = Quaternion.identity;

            espadaActual.transform.localPosition += new Vector3(0.08f, 0.01f, -0.05f);
            espadaActual.transform.localRotation = Quaternion.Euler(-60, 190, 110);
            espadaActual.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
    }
}

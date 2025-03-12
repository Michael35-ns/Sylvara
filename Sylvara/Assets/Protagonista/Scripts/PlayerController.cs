using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 2f;
    public float runSpeed = 4f;
    public float gravity = 9.8f;

    private Vector3 moveDirection;
    private CharacterController controller;
    public Animator animator;

    [Header("Combate y Combo")]
    private int comboIndex = 0;
    private float comboResetTime = 0.5f; // Ajustado para que el combo no se cancele tan rápido
    private bool isAttacking = false;
    private bool comboQueued = false;

    [Header("Sistema de Equipamiento")]
    public bool tieneEspada = false;
    public GameObject espadaPrefab;
    public Transform puntoEspada;
    private GameObject espadaActual;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        HandleAttack();
        ApplyGravity();
    }

    // ---------------- MOVIMIENTO ----------------
    void HandleMovement()
    {
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
        animator.SetFloat("Speed", isAttacking ? 0f : move.magnitude * currentSpeed);

        if (!isAttacking)
        {
            controller.Move(move * currentSpeed * Time.deltaTime);
        }
    }

    // ---------------- ATAQUE ----------------
    void HandleAttack()
    {
        if (!tieneEspada) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking) // Inicia el combo
            {
                comboIndex = 1;
                StartCoroutine(AttackCoroutine());
            }
            else if (comboIndex < 3) // Agregar a la cola si el combo aún no ha terminado
            {
                comboQueued = true;
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        animator.SetInteger("ComboIndex", comboIndex);
        animator.SetTrigger("Attack");

        float animationTime = GetCurrentAnimationLength();
        float elapsedTime = 0f;

        while (elapsedTime < animationTime)
        {
            if (comboQueued && comboIndex < 3) // Si se hizo clic en medio de la animación, preparar el siguiente ataque
            {
                comboQueued = false;
                comboIndex++;
                StartCoroutine(AttackCoroutine());
                yield break; // Detenemos la animación actual y pasamos a la siguiente
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(comboResetTime); // Espera antes de resetear el combo

        EndAttack();
    }

    public void EndAttack()
    {
        isAttacking = false;
        comboIndex = 0;
        comboQueued = false;
        animator.SetInteger("ComboIndex", 0);
    }

    // ---------------- GRAVEDAD ----------------
    void ApplyGravity()
    {
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
        Debug.Log("Gus Gus ha conseguido la espada.");

        if (espadaPrefab != null && puntoEspada != null)
        {
            espadaActual = Instantiate(espadaPrefab, puntoEspada);
            espadaActual.transform.localPosition = Vector3.zero;
            espadaActual.transform.localRotation = Quaternion.identity;

            espadaActual.transform.localPosition += new Vector3(0.08f, 0.01f, -0.05f);
            espadaActual.transform.localRotation = Quaternion.Euler(-60, 190, 110);
            espadaActual.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            Debug.Log("Espada equipada correctamente en la mano.");
        }
    }

    // ---------------- OBTENER DURACIÓN DE ANIMACIÓN ----------------
    float GetCurrentAnimationLength()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length > 0 ? stateInfo.length : 0.1f;
    }
}

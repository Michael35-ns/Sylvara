using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class DashKick : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 8f;
    public float dashDuration = 0.5f;
    public int dashCount = 3;
    public float teleportOffset = 5f;
    public float delayBetweenDashes = 2f;
    public float safeZoneRadius = 5f;

    public float dashDamage = 25f;
    public Collider dashHitbox;
    private HashSet<GameObject> alreadyHit = new HashSet<GameObject>();

    public bool HasAlreadyHit(GameObject obj) => alreadyHit.Contains(obj);
    public void RegisterHit(GameObject obj) => alreadyHit.Add(obj);


    [Header("Visual References")]
    public GameObject modelObject;

    [Header("References")]
    public Animator animator;
    public CharacterController controller;
    private Transform player;

    [Header("Efectos de Teletransporte")]
    public GameObject dashPortalEffect;
    public GameObject dashPreviewLine;
    private LineRenderer lineRenderer;

    private bool isDashing = false;
    private int completedDashes = 0;
    private System.Action onDashComplete;
    private HealthSystem healthSystem;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (animator == null) animator = GetComponent<Animator>();
        if (controller == null) controller = GetComponent<CharacterController>();
        healthSystem = GetComponent<HealthSystem>();
        if (dashPreviewLine != null)
        {
            lineRenderer = dashPreviewLine.GetComponent<LineRenderer>();
            dashPreviewLine.SetActive(false);
        }
        Collider bossCollider = GetComponent<Collider>();
        Collider playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();

        if (bossCollider != null && playerCollider != null)
        {
            Physics.IgnoreCollision(bossCollider, playerCollider);
        }

    }

    public void StartDash(System.Action onComplete = null)
    {
        if (isDashing) return;

        isDashing = true;
        completedDashes = 0;
        onDashComplete = onComplete;

        if (healthSystem != null)
            healthSystem.SetInvulnerable(true);

        alreadyHit.Clear();
        dashHitbox.enabled = true;
        StartCoroutine(ExecuteDashKick());
    }


    private IEnumerator ExtendInvulnerability(float extraTime)
    {
        yield return new WaitForSeconds(extraTime);
        if (healthSystem != null)
            healthSystem.SetInvulnerable(false);
    }

    private IEnumerator ExecuteDashKick()
    {
        while (completedDashes < dashCount)
        {
            dashHitbox.enabled = true;
            alreadyHit.Clear();

            Vector3 teleportPosition;
            int attempts = 0;
            do
            {
                Vector3 teleportDirection = Random.insideUnitSphere * teleportOffset;
                teleportDirection.y = 0;
                teleportPosition = player.position + teleportDirection;
                attempts++;
            }
            while (Vector3.Distance(teleportPosition, player.position) < safeZoneRadius && attempts < 10);

            if (dashPortalEffect != null)
                Instantiate(dashPortalEffect, transform.position, Quaternion.identity);

            if (modelObject != null)
                modelObject.SetActive(false);

            yield return new WaitForSeconds(0.3f); 

            dashHitbox.enabled = false;

            controller.enabled = false;
            transform.position = teleportPosition;
            controller.enabled = true;

            yield return null;

            dashHitbox.enabled = true;

            if (dashPortalEffect != null)
                Instantiate(dashPortalEffect, transform.position, Quaternion.identity);

            if (modelObject != null)
                modelObject.SetActive(true);

            Vector3 lookDir = (player.position - transform.position).normalized;
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.1f)
                transform.rotation = Quaternion.LookRotation(lookDir);

            if (lineRenderer != null)
            {
                dashPreviewLine.SetActive(true);
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, transform.position + transform.forward * 20f);
            }

            float previewTime = 0.5f;
            float t = 0f;
            float maxLength = 20f;

            while (t < previewTime)
            {
                float factor = 1f - (t / previewTime);
                float currentLength = Mathf.Lerp(0f, maxLength, factor);

                if (lineRenderer != null)
                {
                    lineRenderer.SetPosition(0, transform.position);
                    lineRenderer.SetPosition(1, transform.position + transform.forward * currentLength);
                }

                t += Time.deltaTime;
                yield return null;
            }

            // 🔥 Dash animado
            animator.SetTrigger("Kick");

            float elapsed = 0f;
            while (elapsed < dashDuration)
            {
                controller.Move(transform.forward * dashSpeed * Time.deltaTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            dashPreviewLine.SetActive(false);
            completedDashes++;
            
            yield return new WaitForSeconds(delayBetweenDashes);
        }

        isDashing = false;
        dashHitbox.enabled = false;


        if (!controller.enabled)
            controller.enabled = true;

        if (healthSystem != null)
            healthSystem.SetInvulnerable(false);

        onDashComplete?.Invoke();
    }

    public bool IsDashing()
    {
        return isDashing;
    }
}

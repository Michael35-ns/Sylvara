using System.Collections;
using UnityEngine;

public class DeathRayAttack : MonoBehaviour
{
    public GameObject castingIndicatorPrefab;
    public GameObject rayPrefab;
    public GameObject fireballPrefab;

    public Transform raySpawnPoint;
    public Transform fireballSpawnPoint;
    public Transform indicatorFollowPoint;

    public float chargeDuration = 5f;
    public float rayDuration = 15f;
    public float fireballInterval = 1f;
    public float fireballSpeed = 6f;
    public float rotationSpeed = 20f;

    private Transform player;
    private CharacterController controller;
    private Animator animator;
    private bool isCasting = false;
    private System.Action onComplete;

    private GameObject castingIndicator;
    private GameObject rayInstance;
    private GameObject rotationRoot;
    private Coroutine fireballRoutine;
    private Coroutine rotationRoutine;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    public void StartDeathRay(System.Action onCompleteCallback)
    {
        if (isCasting || player == null) return;

        onComplete = onCompleteCallback;
        StartCoroutine(CastDeathRayRoutine());
    }

    public bool IsCasting() => isCasting;

    private IEnumerator CastDeathRayRoutine()
    {
        isCasting = true;

        controller.enabled = false;
        transform.position = new Vector3(0, 1, 0);
        controller.enabled = true;

        animator.SetTrigger("CastDeathRay");

        // Crear raíz para la rotación
        rotationRoot = new GameObject("DeathRayRotationRoot");
        rotationRoot.transform.position = raySpawnPoint.position;
        rotationRoot.transform.rotation = Quaternion.identity;

        // ☢️ Instanciar símbolo y parentarlo desde ya
        castingIndicator = Instantiate(castingIndicatorPrefab);
        castingIndicator.transform.SetParent(rotationRoot.transform);
        castingIndicator.transform.localPosition = Vector3.forward * 1f;
        castingIndicator.transform.localRotation = Quaternion.Euler(180f, 90f, 0f);

        float t = 0f;
        float slowDownTime = 1f;
        float activeTrackingDuration = chargeDuration - slowDownTime;

        while (t < chargeDuration)
        {
            if (indicatorFollowPoint)
            {
                Vector3 toPlayer = player.position - indicatorFollowPoint.position;
                toPlayer.y = 0;

                if (toPlayer.sqrMagnitude > 0.1f)
                {
                    float speedFactor = (t < activeTrackingDuration) ? 3f : 0.5f;
                    Quaternion targetRot = Quaternion.LookRotation(toPlayer.normalized);
                    indicatorFollowPoint.rotation = Quaternion.Slerp(indicatorFollowPoint.rotation, targetRot, Time.deltaTime * speedFactor);

                    Vector3 bossDir = player.position - transform.position;
                    bossDir.y = 0;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(bossDir), Time.deltaTime * speedFactor);
                }

                rotationRoot.transform.rotation = indicatorFollowPoint.rotation;
            }

            t += Time.deltaTime;
            yield return null;
        }

        // ⚡ Instanciar rayo y parentarlo al mismo root
        if (rayPrefab && raySpawnPoint)
        {
            rayInstance = Instantiate(rayPrefab, raySpawnPoint.position, indicatorFollowPoint.rotation);
            rayInstance.transform.SetParent(rotationRoot.transform);
            rayInstance.transform.localPosition = Vector3.zero;
            Destroy(rayInstance, rayDuration);
        }

        // 🔄 Comenzar rotación sincronizada
        rotationRoutine = StartCoroutine(RotateBossAndRay());

        // 🔥 Lanzar bolas de fuego
        fireballRoutine = StartCoroutine(SpawnChasingFireballs());

        yield return new WaitForSeconds(rayDuration);

        if (fireballRoutine != null) StopCoroutine(fireballRoutine);
        if (rotationRoutine != null) StopCoroutine(rotationRoutine);
        if (castingIndicator != null) Destroy(castingIndicator);
        if (rotationRoot != null) Destroy(rotationRoot);

        isCasting = false;
        onComplete?.Invoke();
    }

    private IEnumerator RotateBossAndRay()
    {
        while (true)
        {
            if (player == null || rotationRoot == null) yield break;

            Vector3 toPlayer = player.position - rotationRoot.transform.position;
            toPlayer.y = 0;

            if (toPlayer.sqrMagnitude > 0.1f)
            {
                Quaternion targetRot = Quaternion.LookRotation(toPlayer);
                rotationRoot.transform.rotation = Quaternion.RotateTowards(rotationRoot.transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            yield return null;
        }
    }

    private IEnumerator SpawnChasingFireballs()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireballInterval);

            if (fireballPrefab && fireballSpawnPoint)
            {
                var fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
                fireball.GetComponent<FireballProjectile>().SetTarget(player, fireballSpeed);
            }
        }
    }
}

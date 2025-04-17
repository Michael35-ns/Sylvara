using System.Collections;
using UnityEngine;

public class FireballAttack : MonoBehaviour
{
    [Header("Fireball Settings")]
    public GameObject fireballPrefab;
    public Transform fireballSpawnPoint;

    [Header("Casteo Visual")]
    public GameObject castEffectPrefab;
    public Transform castEffectSpawnPoint;

    private Transform player;
    private Animator animator;
    private System.Action onCastComplete;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void CastFireball(System.Action callback = null)
    {
        if (player == null || fireballPrefab == null || fireballSpawnPoint == null) return;

        onCastComplete = callback;
        StartCoroutine(CastFireballRoutine());
    }

    private IEnumerator CastFireballRoutine()
    {
        // ⛔ No moverse ni teleportarse durante el casteo
        GetComponent<BossController>().SetCasting(true);

        animator.SetBool("Walking", false);
        animator.SetBool("Running", false);
        animator.SetBool("WalkingBackwards", false);
        animator.SetTrigger("CastFireball");

        if (castEffectPrefab && castEffectSpawnPoint)
            Instantiate(castEffectPrefab, castEffectSpawnPoint.position, Quaternion.identity);

        yield return new WaitForSeconds(1.5f); // ⏳ Tiempo de casteo extendido

        if (fireballPrefab && fireballSpawnPoint)
        {
            Vector3 spawnPosition = fireballSpawnPoint.position + Vector3.up * 1.2f; // o ajusta el valor como prefieras
            var fireball = Instantiate(fireballPrefab, spawnPosition, Quaternion.identity);

            fireball.GetComponent<FireballProjectile>().SetTarget(player);
        }

        yield return new WaitForSeconds(0.2f);

        onCastComplete?.Invoke();

        // ✅ Permitir moverse de nuevo
        GetComponent<BossController>().SetCasting(false);
    }

}

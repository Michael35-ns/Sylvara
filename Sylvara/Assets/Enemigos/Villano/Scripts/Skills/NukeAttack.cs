using System.Collections;
using UnityEngine;

public class NukeAttack : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject castEffectPrefab;
    public GameObject explosionEffectPrefab;

    [Header("Positions")]
    public Transform nukeCenter;
    public Collider damageArea;

    [Header("Stats")]
    public float chargeTime = 7f;
    public float nukeDamage = 70f;

    private bool isCasting = false;
    private bool hasTakenHit = false;
    private Transform player;
    private Animator animator;
    private System.Action onFinished;

    private GameObject activeCastEffect;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        damageArea.enabled = false;
    }

    public void StartNuke(System.Action callback)
    {
        if (isCasting || player == null) return;
        onFinished = callback;
        StartCoroutine(CastNukeRoutine());
    }

    private IEnumerator CastNukeRoutine()
    {
        isCasting = true;
        hasTakenHit = false;

        // Teleport cerca del jugador
        Vector3 teleportDirection = Random.insideUnitSphere;
        teleportDirection.y = 0;
        teleportDirection.Normalize();
        Vector3 position = player.position + teleportDirection * 3f;
        position.y = transform.position.y;

        GetComponent<CharacterController>().enabled = false;
        transform.position = position;
        GetComponent<CharacterController>().enabled = true;

        // Mirar al jugador
        Vector3 lookDir = (player.position - transform.position).normalized;
        lookDir.y = 0;
        if (lookDir.sqrMagnitude > 0.1f)
            transform.rotation = Quaternion.LookRotation(lookDir);

        // Animación de casteo
        animator.SetTrigger("CastNuke");

        // ⚡ Efecto de casteo (area circular)
        if (castEffectPrefab && nukeCenter)
        {
            activeCastEffect = Instantiate(castEffectPrefab, nukeCenter.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(chargeTime);

        // 🧨 Efecto de explosión (visual boom)
        if (explosionEffectPrefab && nukeCenter)
        {
            Instantiate(explosionEffectPrefab, nukeCenter.position, Quaternion.identity);
        }

        // 🔥 Habilita daño por corto tiempo
        damageArea.enabled = true;
        yield return new WaitForSeconds(0.1f);
        damageArea.enabled = false;

        // Limpieza del área de casteo si no se destruye sola
        if (activeCastEffect)
        {
            Destroy(activeCastEffect);
        }

        isCasting = false;
        onFinished?.Invoke();
    }

    public bool IsCasting() => isCasting;

    public bool CanTakeHit()
    {
        if (hasTakenHit) return false;
        hasTakenHit = true;
        return true;
    }
}

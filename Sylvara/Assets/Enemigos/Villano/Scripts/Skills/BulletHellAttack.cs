using System.Collections;
using UnityEngine;

public class BulletHellAttack : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform spawnCenter;
    public GameObject chargeEffectPrefab;

    public int bulletsPerWave = 24;
    public float bulletSpeedFast = 12f;
    public float bulletSpeedSlow = 5f;
    public float waveInterval = 0.5f;
    public float durationPhase1 = 10f;
    public float durationPhase2 = 15f;

    private Transform player;
    private CharacterController controller;
    private Animator animator;
    private bool isCasting = false;
    private System.Action onComplete;
    private Coroutine activeRoutine;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    public void StartBulletHell(bool phase2, System.Action callback)
    {
        if (isCasting) return;

        onComplete = callback;
        activeRoutine = StartCoroutine(BulletHellRoutine(phase2));
    }

    public bool IsCasting() => isCasting;

    private IEnumerator BulletHellRoutine(bool phase2)
    {
        isCasting = true;
        float duration = phase2 ? durationPhase2 : durationPhase1;
        float endTime = Time.time + duration;

        animator.SetTrigger("CastBulletHell");

        // ⏺️ Instanciar efecto visual si existe
        if (chargeEffectPrefab && spawnCenter)
        {
            Instantiate(chargeEffectPrefab, spawnCenter.position, Quaternion.identity, transform);
        }

        controller.enabled = false;
        transform.position = new Vector3(0, 1, 0);
        controller.enabled = true;

        while (Time.time < endTime)
        {
            FireBulletWave(phase2);
            RotateTowardsPlayer();
            yield return new WaitForSeconds(waveInterval);
        }

        isCasting = false;
        onComplete?.Invoke();
    }

    private void FireBulletWave(bool complex)
    {
        float angleStep = 360f / bulletsPerWave;
        float angle = Random.Range(0f, 360f);

        for (int i = 0; i < bulletsPerWave; i++)
        {
            float bulletAngle = angle + angleStep * i;
            float rad = bulletAngle * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad));

            GameObject bullet = Instantiate(bulletPrefab, spawnCenter.position + Vector3.up * 0.5f, Quaternion.LookRotation(dir));
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
                rb.velocity = dir * (complex && i % 2 == 0 ? bulletSpeedFast : bulletSpeedSlow);
        }
    }

    private void RotateTowardsPlayer()
    {
        if (!player) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0;
        if (toPlayer.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(toPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 3f);
        }
    }
}

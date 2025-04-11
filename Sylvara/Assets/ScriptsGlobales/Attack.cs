using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("Parámetros del Ataque")]
    public float damage = 20f;
    public float attackRadius = 1.5f;
    public float attackRange = 2f;
    public float activeTime = 0.1f;

    [SerializeField] private LayerMask attackableLayers;

    private GameObject owner;
    private bool attackActive = false;

    void Start()
    {
        if (attackableLayers == 0)
        {
            attackableLayers = LayerMask.GetMask("Enemy");
        }
    }

    public void SetOwner(GameObject attacker)
    {
        owner = attacker;
    }

    public void PerformAttack()
    {
        if (attackActive || owner == null) return;

        attackActive = true;

        Vector3 attackCenter = owner.transform.position + owner.transform.forward * (attackRange * 0.5f);
        Vector3 boxHalfSize = new Vector3(attackRadius, 1f, attackRange * 0.5f);

        Collider[] hitColliders = Physics.OverlapBox(
            attackCenter,
            boxHalfSize,
            owner.transform.rotation,
            attackableLayers,
            QueryTriggerInteraction.Collide
        );

        List<string> hits = new List<string>();

        foreach (Collider col in hitColliders)
        {
            if (col.gameObject == owner) continue;

            HealthSystem health = col.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(damage);
                hits.Add(col.gameObject.name);
            }
        }

        Debug.Log(hits.Count > 0
            ? $"🔥 {owner.name} golpeó a: {string.Join(", ", hits)}"
            : "💨 El ataque no golpeó a nadie.");

        Invoke(nameof(DisableHitbox), activeTime);
    }

    private void DisableHitbox()
    {
        attackActive = false;
    }

    private void OnDrawGizmos()
    {
        if (!attackActive || owner == null) return;

        Gizmos.color = Color.red;
        Vector3 center = owner.transform.position + owner.transform.forward * (attackRange * 0.5f);
        Quaternion rotation = Quaternion.LookRotation(owner.transform.forward);
        Vector3 size = new Vector3(attackRadius * 2, 2f, attackRange);

        Matrix4x4 matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        Gizmos.matrix = matrix;
        Gizmos.DrawWireCube(Vector3.zero, size);
    }
}

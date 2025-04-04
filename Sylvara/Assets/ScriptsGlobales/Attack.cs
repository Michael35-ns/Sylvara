using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
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
        if (attackActive) return;
        attackActive = true;

        Vector3 attackCenter = owner.transform.position + owner.transform.forward * (attackRange * 0.5f);
        Quaternion attackRotation = Quaternion.LookRotation(owner.transform.forward);

        Vector3 boxSize = new Vector3(attackRadius * 2, 2f, attackRange);
        Collider[] hitEnemies = Physics.OverlapBox(attackCenter, new Vector3(attackRadius, 1f, attackRadius), owner.transform.rotation, attackableLayers, QueryTriggerInteraction.Collide);

        List<string> hitNames = new List<string>();

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.gameObject == owner) continue;

            HealthSystem health = enemy.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(damage);
                hitNames.Add(enemy.gameObject.name);
            }
        }

        if (hitNames.Count > 0)
        {
            Debug.Log($"🔥 {owner.name} golpeó a: {string.Join(", ", hitNames)}");
        }
        else
        {
            Debug.Log("💨 El ataque no golpeó a nadie.");
        }

        Invoke("DisableHitbox", activeTime);
    }

    private void DisableHitbox()
    {
        attackActive = false;
    }

    private void OnDrawGizmos()
    {
        if (owner == null || !attackActive) return;

        Gizmos.color = Color.red;
        Vector3 attackCenter = owner.transform.position + owner.transform.forward * (attackRange * 0.5f);
        Quaternion attackRotation = Quaternion.LookRotation(owner.transform.forward);
        Vector3 boxSize = new Vector3(attackRadius * 2, 2f, attackRange);

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(attackCenter, attackRotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }

}

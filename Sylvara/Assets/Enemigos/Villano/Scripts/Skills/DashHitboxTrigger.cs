using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashHitboxTrigger : MonoBehaviour
{
    private DashKick dashKick;

    void Start()
    {
        dashKick = GetComponentInParent<DashKick>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"➡️ Trigger con {other.name}");

        if (dashKick == null || !dashKick.IsDashing()) return;

        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<HealthSystem>();
            if (health != null && !dashKick.HasAlreadyHit(other.gameObject))
            {
                Debug.Log("💥 ¡Daño aplicado!");
                health.TakeDamage(dashKick.dashDamage);
                dashKick.RegisterHit(other.gameObject);
            }
        }
    }

}

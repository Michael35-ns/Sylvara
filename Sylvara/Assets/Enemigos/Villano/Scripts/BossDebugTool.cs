using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDebugTool : MonoBehaviour
{
    public HealthSystem bossHealth;

    [Tooltip("Tecla para activar el modo de prueba del Death Ray (baja vida del jefe)")]
    public KeyCode triggerKey = KeyCode.L;

    void Update()
    {
        if (Input.GetKeyDown(triggerKey) && bossHealth != null)
        {
            float newHealth = bossHealth.maxHealth * 0.10f;
            Debug.Log($"🔻 Vida del jefe forzada a {newHealth}");
            bossHealth.TakeDamage(bossHealth.GetHealth() - newHealth);
        }
    }
}

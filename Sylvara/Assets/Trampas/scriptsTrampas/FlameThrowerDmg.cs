using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerDmg : MonoBehaviour
{
    public float damagePerHit = 10f;

    private void OnParticleCollision(GameObject other)
    {
        // Verifica si el objeto tiene el sistema de salud
        HealthSystem health = other.GetComponent<HealthSystem>();

        if (health != null)
        {
            health.TakeDamage(damagePerHit);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDmg : MonoBehaviour
{
    public float damage = 20f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Algo entró en la trampa: " + other.name);

        // Verificar que el objeto con el trigger tiene el tag "Player"
        if (other.CompareTag("Player"))
        {
            Debug.Log("Es el jugador, aplicando daño...");
            HealthSystem health = other.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("El jugador no tiene componente HealthSystem.");
            }
        }
    }
}

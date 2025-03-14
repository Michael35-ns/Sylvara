using UnityEngine;

public class DebugDamage : MonoBehaviour
{
    public HealthSystem healthSystem; // Asigna en el Inspector
    public float testDamage = 10f;
    public float testHeal = 5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) // 🔹 Presiona H para hacer daño
        {
            if (healthSystem != null)
            {
                healthSystem.TakeDamage(testDamage);
                Debug.Log($"⚠️ GusGus recibió {testDamage} de daño.");
            }
        }

        if (Input.GetKeyDown(KeyCode.J)) // 🔹 Presiona J para curarlo
        {
            if (healthSystem != null)
            {
                healthSystem.Heal(testHeal);
                Debug.Log($"🩹 GusGus se curó {testHeal} puntos de vida.");
            }
        }
    }
}

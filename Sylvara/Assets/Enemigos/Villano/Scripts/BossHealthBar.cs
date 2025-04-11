using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject healthBarRoot; // Para ocultar todo si el jefe muere

    private HealthSystem bossHealth;

    public void SetBoss(HealthSystem healthSystem)
    {
        bossHealth = healthSystem;
        if (healthBarRoot != null)
            healthBarRoot.SetActive(true);
    }

    private void Update()
    {
        if (bossHealth == null) return;

        float ratio = Mathf.Clamp01(bossHealth.GetHealth() / bossHealth.maxHealth);
        fillImage.fillAmount = ratio;

        if (bossHealth.GetHealth() <= 0f && healthBarRoot != null)
        {
            healthBarRoot.SetActive(false); // Oculta la barra al morir el jefe
        }
    }
}

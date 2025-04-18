using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public GameObject defeatPanel;
    public GameObject victoryPanel;
    public Transform respawnPoint;

    void Start()
    {
        if (defeatPanel != null) defeatPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    public void ShowDefeatScreen()
    {
        if (defeatPanel != null) defeatPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowVictoryScreen()
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && respawnPoint != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.transform.position = respawnPoint.position;

            if (cc != null) cc.enabled = true;

            // Reset vida
            HealthSystem health = player.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.Heal(health.maxHealth);
                health.isDead = false;
            }

            defeatPanel.SetActive(false);
        }
    }

    public void Continue()
    {
        Time.timeScale = 1f;
        Debug.Log("⚠️ Continúa desde victoria (a definir)");
    }
}

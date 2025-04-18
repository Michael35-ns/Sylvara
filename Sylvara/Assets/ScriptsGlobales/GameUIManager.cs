using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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
        if (player != null)
        {
            PlayerCheckpoint checkpoint = player.GetComponent<PlayerCheckpoint>();
            if (checkpoint != null)
                checkpoint.Respawn();

            HealthSystem health = player.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.ReactivateComponents();
                health.Heal(health.maxHealth);
                health.SetInvulnerable(true);
                StartCoroutine(RemoveInvulnerability(health, 2f));
            }

            defeatPanel.SetActive(false);
        }
    }

    private IEnumerator RemoveInvulnerability(HealthSystem health, float delay)
    {
        yield return new WaitForSeconds(delay);
        health.SetInvulnerable(false);
    }


    public void Continue()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

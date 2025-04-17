using UnityEngine;

public class ActivarBoss : MonoBehaviour
{
    public GameObject villano;         // Referencia al enemigo
    public GameObject barraDeVida;     // Referencia a la healthbar (canvas, etc.)

    private bool activado = false;

    private void Start()
    {
        if (villano != null)
            villano.SetActive(false);

        if (barraDeVida != null)
            barraDeVida.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!activado && other.CompareTag("Player"))
        {
            if (villano != null)
                villano.SetActive(true);

            if (barraDeVida != null)
                barraDeVida.SetActive(true);

            activado = true;
            Debug.Log("Villano y barra de vida activados.");
        }
    }
}

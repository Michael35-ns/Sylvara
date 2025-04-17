using System.Collections;
using UnityEngine;
using TMPro;

public class NpcController : MonoBehaviour
{
    public string mensajeDialogo = "Sigue así, vas por buen camino.";
    public Animator animator;
    private bool jugadorCerca = false;

    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;  // <--- cambiado aquí
    public float duracionDialogo = 3f;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null)
            animator.Play("Idle");

        if (panelDialogo != null)
            panelDialogo.SetActive(false);
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            MostrarDialogo();
        }
    }

    void MostrarDialogo()
    {
        if (panelDialogo != null && textoDialogo != null)
        {
            textoDialogo.text = mensajeDialogo;
            panelDialogo.SetActive(true);
            StartCoroutine(DesactivarDialogoDespuesDeTiempo());
        }
    }

    IEnumerator DesactivarDialogoDespuesDeTiempo()
    {
        yield return new WaitForSeconds(duracionDialogo);
        panelDialogo.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            Debug.Log("Presiona 'E' para hablar.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }
}

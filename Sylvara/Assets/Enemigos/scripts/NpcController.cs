using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Necesario para usar UI

public class NpcController : MonoBehaviour
{
    public string mensajeDialogo = "Sigue así, vas por buen camino.";
    public Animator animator;
    private bool jugadorCerca = false;

    public GameObject panelDialogo; // Referencia al panel que contiene el texto
    public Text textoDialogo; // Texto que se mostrará en el panel
    public float duracionDialogo = 3f; // Tiempo que se muestra el mensaje

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null)
            animator.Play("Idle");

        if (panelDialogo != null)
            panelDialogo.SetActive(false); // Ocultamos el panel al inicio
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


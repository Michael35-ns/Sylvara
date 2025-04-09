using UnityEngine;

public class ChangeLightColor : MonoBehaviour
{
    public Light sceneLight; // La luz que quieres cambiar
    public Color targetColor = Color.blue; // El color al que quieres que cambie la luz
    public Color originalColor = Color.white; // El color original de la luz
    public string playerTag = "Player"; // Etiqueta que debe tener el personaje

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que entra en el trigger es el personaje
        if (other.CompareTag(playerTag))
        {
            // Cambiar el color de la luz
            if (sceneLight != null)
            {
                sceneLight.color = targetColor;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Mientras el personaje esté dentro de la zona, mantén el color de la luz
        if (other.CompareTag(playerTag))
        {
            if (sceneLight != null)
            {
                sceneLight.color = targetColor;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Cuando el personaje salga de la zona, vuelve a su color original
        if (other.CompareTag(playerTag))
        {
            if (sceneLight != null)
            {
                sceneLight.color = originalColor;
            }
        }
    }
}

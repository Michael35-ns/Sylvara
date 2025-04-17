using UnityEngine;

public class ChangeLightColor : MonoBehaviour
{
    public Light sceneLight; // La luz que quieres cambiar
    public Color targetColor = Color.blue; // El color al que quieres que cambie la luz
    public float targetIntensity = 5.0f; // La intensidad a aplicar
    public float originalIntensity = 1.0f; // Intensidad original
    public Color originalColor = Color.white; // El color original de la luz
    public string playerTag = "Player"; // Etiqueta que debe tener el personaje

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (sceneLight != null)
            {
                sceneLight.color = targetColor;
                sceneLight.intensity = targetIntensity;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (sceneLight != null)
            {
                sceneLight.color = targetColor;
                sceneLight.intensity = targetIntensity;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (sceneLight != null)
            {
                sceneLight.color = originalColor;
                sceneLight.intensity = originalIntensity;
            }
        }
    }
}

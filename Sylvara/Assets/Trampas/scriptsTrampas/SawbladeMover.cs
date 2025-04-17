using UnityEngine;

public class SawbladeMover : MonoBehaviour
{
    private Transform Sawblade;  // Referencia al objeto Sawblade (hijo)
    public float rotationSpeed = 200f;  // Velocidad de rotación
    public float moveSpeed = 3f;        // Velocidad de movimiento
    public float moveDistance = 2f;     // Distancia de movimiento

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; // Guarda la posición inicial del padre

        // Depuración: Mostrar el nombre del objeto padre
        Debug.Log("Nombre del objeto padre: " + gameObject.name);

        // Buscar el objeto "Sawblade" dentro del padre
        Sawblade = transform.Find("Sawblade");

        // Si no se encuentra, buscar en toda la escena (alternativa)
        if (Sawblade == null)
        {
            GameObject foundSawblade = GameObject.Find("Sawblade");

            if (foundSawblade != null)
            {
                Sawblade = foundSawblade.transform;
            }
        }
    }

    void Update()
    {
        // Movimiento en línea recta del objeto padre (Traphome_Mid)
        float offset = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        transform.position = startPosition + new Vector3(offset, 0, 0); // Movimiento en X

        // Rotación de la sierra (Sawblade)
        if (Sawblade != null)
        {
            Sawblade.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}

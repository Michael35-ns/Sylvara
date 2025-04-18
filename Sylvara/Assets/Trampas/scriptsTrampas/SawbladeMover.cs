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
        startPosition = transform.position;

        // Buscar el objeto "Sawblade" dentro del objeto actual
        Sawblade = transform.Find("Sawblade");

        if (Sawblade == null)
        {
            Debug.LogError("No se encontró 'Sawblade' como hijo de " + gameObject.name);
        }
    }

    void Update()
    {
        // Movimiento en línea recta del objeto padre (Traphome_Mid)
        float offset = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
        transform.position = startPosition + new Vector3(offset, 0, 0); // Movimiento en X

        if (Sawblade != null)
        {
            Sawblade.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}

using UnityEngine;

public class SawbladeMover : MonoBehaviour
{
    private Transform Sawblade;  
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
        if (Sawblade != null)
        {
            // Rotar la sierra
            Sawblade.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);

            // Mover la sierra en el eje X
            float offset = Mathf.PingPong(Time.time * moveSpeed, moveDistance);
            Sawblade.localPosition = new Vector3(offset, Sawblade.localPosition.y, Sawblade.localPosition.z);
        }
    }
}

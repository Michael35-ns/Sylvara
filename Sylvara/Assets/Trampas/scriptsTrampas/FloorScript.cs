using UnityEngine;

public class FloorScript : MonoBehaviour
{
    public float moveSpeed = 2f;      // Velocidad de movimiento
    public float moveDistance = 3f;   // Distancia que recorrer� la plataforma

    private Vector3 startPosition;    // Posici�n inicial de la plataforma

    void Start()
    {
        startPosition = transform.position; // Guarda la posici�n inicial
    }

    void Update()
    {
        // Movimiento de izquierda a derecha usando PingPong
        float offset = Mathf.PingPong(Time.time * moveSpeed, moveDistance * 2) - moveDistance;
        transform.position = startPosition + new Vector3(offset, 0, 0);
    }
}

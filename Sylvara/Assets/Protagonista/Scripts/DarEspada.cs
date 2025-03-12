using UnityEngine;

public class DarEspada : MonoBehaviour
{
    public GameObject espadaPrefab; // Espada que entregará

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.espadaPrefab = espadaPrefab; // Le asigna la espada que debe instanciar
                player.ObtenerEspada();
                Destroy(gameObject); // Se destruye después de dar la espada
            }
        }
    }
}

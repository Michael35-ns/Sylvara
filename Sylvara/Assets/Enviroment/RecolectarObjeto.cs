using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecolectarObjeto : MonoBehaviour
{
    public GameObject puertaIzquierda;
    public GameObject puertaDerecha;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Desactivar las puertas
            puertaIzquierda.SetActive(false);
            puertaDerecha.SetActive(false);

            // Destruir el objeto recolectable
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorretaScript : MonoBehaviour
{
    public float rotationSpeed = 30f; // Velocidad de rotaci�n en grados por segundo
    private ParticleSystem fireParticles;

    void Start()
    {
        // Busca el sistema de part�culas en cualquier hijo de "Torreta"
        fireParticles = GetComponentInChildren<ParticleSystem>();

        if (fireParticles == null)
        {
            Debug.LogError("No se encontr� un Particle System en los hijos de Torreta.");
        }
    }

    void Update()
    {
        // Gira solo sobre su propio eje (sin moverse de lugar)
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);

        // Si el sistema de part�culas fue encontrado, manejar su activaci�n
        if (fireParticles != null)
        {
            // Asegurarse de que el hijo siga la rotaci�n del padre sin desviarse
            fireParticles.transform.rotation = transform.rotation;

            fireParticles.Play();
        }
    }
}
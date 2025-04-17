using UnityEngine;
using System.Collections;

public class csDestroyEffect : MonoBehaviour {

    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        // Por seguridad, si no hay ps, destruimos luego de 5s por defecto
        if (ps == null)
        {
            Destroy(gameObject, 10f);
        }
    }

    void Update()
    {
        // Destruir solo si el sistema ya no está vivo (incluye subemisores)
        if (ps != null && !ps.IsAlive(true))
        {
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NukeTrigger : MonoBehaviour
{
    private NukeAttack nukeAttack;

    void Start()
    {
        nukeAttack = GetComponentInParent<NukeAttack>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (nukeAttack == null || !nukeAttack.IsCasting()) return;

        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<HealthSystem>();
            if (health != null)
            {
                health.TakeDamage(nukeAttack.nukeDamage);
                Debug.Log("💣 ¡Explosión N.U.K.E. alcanzó al jugador!");
            }
        }
    }
}
